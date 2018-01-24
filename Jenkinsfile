node('master', {
        
    try{
            stage("scm pull") {
				deleteDir();
				cloneRepo();
               
            }

            stage ("dotnet build") {
				dotnet_build();
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
