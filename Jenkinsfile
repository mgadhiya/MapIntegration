node('master', {
        
    try{
            stage("scm pull") {
				deleteDir();
				cloneRepo();
               
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
        sshell(script: 'tar zcf netcoreapp.tar.gz -C ./obj/Docker/publish .', returnStdout: true);
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

def docker_run(){
    dir('MapSolution/MapSolution') {
        def containerId = createContainer();
        renameContainer(containerId);
        startContainer();
    }
}
def startContainer(){
    dockerApiRequest('containers/netcoreapp/start', 'POST');
}

def renameContainer(containerId){
    def request = 'containers/' + containerId + '/rename?name=netcoreapp';
    dockerApiRequest(request, 'POST');
}
