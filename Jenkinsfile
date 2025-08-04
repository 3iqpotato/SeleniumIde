pipeline {
    agent {
        docker {
            image 'selenium-dotnet-tests'  // Нашия custom image
            args '-v /var/run/docker.sock:/var/run/docker.sock -u root --shm-size=2g'
        }
    }

    stages {
        stage('Build & Test') {
            steps {
                sh '''
                export DISPLAY=:99
                Xvfb :99 -screen 0 1280x1024x16 -ac &
                
                dotnet restore
                dotnet build
                dotnet test --logger "trx;LogFileName=testresults.trx" --blame
                '''
            }
        }
    }

    post {
        always {
            junit '**/TestResults/*.trx'
            archiveArtifacts artifacts: '**/TestResults/*.trx', allowEmptyArchive: true
        }
    }
}