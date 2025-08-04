pipeline {
    agent any

    environment {
        DOTNET_VERSION = '6.0'
        CHROMEDRIVER_VERSION = '114.0.5735.90'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Setup .NET') {
            steps {
                script {
                    // Изтегляне и инсталиране на .NET без системни пакети
                    sh '''
                    curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version ${env.DOTNET_VERSION}
                    export PATH="$PATH:$HOME/.dotnet"
                    dotnet --version
                    '''
                }
            }
        }

        stage('Setup ChromeDriver') {
            steps {
                script {
                    // Изтегляне на ChromeDriver директно в workspace
                    sh '''
                    curl -Lo chromedriver.zip https://chromedriver.storage.googleapis.com/${env.CHROMEDRIVER_VERSION}/chromedriver_linux64.zip
                    unzip -o chromedriver.zip
                    chmod +x chromedriver
                    export PATH="$PATH:$(pwd)"
                    '''
                }
            }
        }

        stage('Build and Test') {
            steps {
                sh '''
                export PATH="$PATH:$HOME/.dotnet:$(pwd)"
                dotnet restore
                dotnet build --configuration Release
                dotnet test --logger "trx;LogFileName=TestResults.trx"
                '''
            }
        }
    }

    post {
        always {
            archiveArtifacts artifacts: '**/TestResults/*.trx', allowEmptyArchive: true
            junit '**/TestResults/*.trx'
        }
    }
}