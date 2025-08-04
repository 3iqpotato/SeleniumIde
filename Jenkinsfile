pipeline {
    agent any
    
    stages {
        stage('Prepare') {
            agent {
                docker {
                    image 'selenium-dotnet-tests'
                    args '-v /var/run/docker.sock:/var/run/docker.sock -u root --shm-size=2g'
                    reuseNode true
                }
            }
            steps {
                sh 'dotnet restore'
            }
        }
        
        stage('Build') {
            agent {
                docker {
                    image 'selenium-dotnet-tests'
                    args '-v /var/run/docker.sock:/var/run/docker.sock -u root --shm-size=2g'
                    reuseNode true
                }
            }
            steps {
                sh 'dotnet build --no-restore'
            }
        }
        
        stage('Test') {
            agent {
                docker {
                    image 'selenium-dotnet-tests'
                    args '-v /var/run/docker.sock:/var/run/docker.sock -u root --shm-size=2g'
                    reuseNode true
                }
            }
            steps {
                sh '''
                export DISPLAY=:99
                Xvfb :99 -screen 0 1280x1024x16 -ac &
                dotnet test --no-build --logger "trx;LogFileName=testresults.trx" --blame
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