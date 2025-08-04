pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Setup .NET') {
            steps {
                script {
                    // Проверка дали .NET вече е инсталиран
                    def dotnetInstalled = sh(returnStatus: true, script: 'command -v dotnet') == 0
                    
                    if (!dotnetInstalled) {
                        // Инсталиране на .NET 6
                        sh '''
                        wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
                        sudo dpkg -i packages-microsoft-prod.deb
                        rm packages-microsoft-prod.deb
                        sudo apt-get update
                        sudo apt-get install -y dotnet-sdk-6.0
                        '''
                    }
                    
                    sh 'dotnet --version'
                }
            }
        }

        stage('Build and Test') {
            steps {
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release'
                sh 'dotnet test --logger "trx;LogFileName=TestResults.trx"'
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