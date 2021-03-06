import groovy.json.JsonSlurper
import java.lang.String
import java.util.ArrayList

VERSION_NUMBER = ""
node('master', {
        
    try{
            stage("scm pull") {
				deleteDir();
				cloneRepo();
		    VERSION_NUMBER = getVersionNumber();
                currentBuild.displayName = "$VERSION_NUMBER";
               
            }

            stage ("dotnet build") {
				dotnet_build();
            }
	    stage ("dotnet publish") {
				dotnet_publish();
            }

            stage ("docker build") {
				docker_build();
            }

            stage ("docker run") {
				docker_run();
            }


        }
        catch (InterruptedException x) {
            currentBuild.result = 'ABORTED';
            throw x;
        }
        catch (e) {
            currentBuild.result = 'FAILURE';
            throw e;
        }
   })

def cloneRepo() {
    checkout scm;
}

def dotnet_build(){
	dir('MapSolution/MapSolution') {
		shell(script: 'dotnet build MapSolution.csproj', returnStdout: true);
	}
}

def dotnet_publish(){
    dir('MapSolution/MapSolution') {
        shell(script: 'dotnet publish MapSolution.csproj -o ./obj/Docker/publish', returnStdout: true);
        shell(script: 'cp Dockerfile ./obj/Docker/publish', returnStdout: true);
        shell(script: 'tar zcf netcoreapp.tar.gz -C ./obj/Docker/publish .', returnStdout: true);
    }
}

def docker_build(){
    dir('MapSolution/MapSolution') {
        dockerApiRequest('containers/netcoreapp/stop', 'POST');
        dockerApiRequest('containers/prune', 'POST');
        dockerApiRequest('images/netcoreapp', 'DELETE');
        dockerApiRequest('build?t=netcoreapp:' + VERSION_NUMBER + '&nocache=1&rm=1', 'POST', 'tar','', '@netcoreapp.tar.gz', true);
    }
}
def createContainer(){
    shell('echo \'{ "Image": "netcoreapp:' + VERSION_NUMBER + '", "ExposedPorts": { "5000/tcp" : {} }, "HostConfig": { "PortBindings": { "5000/tcp": [{ "HostPort": "5000" }] } } }\' > imageconf');
println 'containerId : '+ containerId;
    def createResponse = dockerApiRequest('containers/create', 'POST', 'json', 'json', '@imageconf');
    def containerId = createResponse.Id;

    return containerId;
}

def docker_run(){
    dir('MapSolution/MapSolution') {
        def containerId = createContainer();
	    println 'containerId : '+ containerId;
        renameContainer(containerId);
        startContainer();
    }
}
def startContainer(){
    dockerApiRequest('containers/netcoreapp/start', 'POST');
}

def renameContainer(containerId){
    def request = 'containers/' + containerId + '/rename?name=netcoreapp';
	   println 'rename containerId : ';
    dockerApiRequest(request, 'POST');
}

//Generates a version number
def getVersionNumber() {
    def out = shell(script: 'git rev-list --count HEAD', returnStdout: true);
 out = "A\n\nB\n\n\nC"
    String[] array = out.split('\n');
	
	println array;
    def count = array[array.length - 1];

    def commitCount = count.trim();

    return commitCount;
}

def dockerApiRequest(request, method, contenttype = 'json', accept = '', data = '', isDataBinary = false){
    def requestBuilder = 'curl -v -X ' + method + ' --unix-socket /var/run/docker.sock:/tmp/docker.sock "tcp://localhost:2375/' + request + '"';

    if(contenttype == 'json'){
        requestBuilder += ' -H "Content-Type:application/json"';
    }

    if(contenttype == 'tar'){
        requestBuilder += ' -H "Content-Type:application/x-tar"';
    }

    if(accept == 'json'){
        requestBuilder += ' -H "Accept: application/json"';
    }
    
    if(data.trim()){
        if(isDataBinary){
            requestBuilder += ' --data-binary ' + data + ' --dump-header - --no-buffer';
        }else{
            requestBuilder += ' -d ' + data;
        }
    }

    def response = shell(script: requestBuilder, returnStdout:true);
    println 'response : '+ response;
	
	println 'request : '+ requestBuilder;
    if(accept == 'json'){
        def jsonSlurper = new JsonSlurper();
        def json = jsonSlurper.parseText('{"result":[{"id":"167687","dapadmin":"false","status":"in progress","lastupdated":"2017-04-21 14:34:30.0","started":"2017-04-21 14:34:28.0","user":"sys","log":"Running a Stop action\n\nRunning command \n"}]}');
        return json;
    }

    return null;
}
