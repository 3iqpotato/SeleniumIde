pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Install .NET 8') {
            steps {
                sh '''
                # Проверка дали .NET вече е инсталиран
                if ! command -v dotnet &> /dev/null; then
                    echo "Инсталиране на .NET 8..."
                    curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 8.0.100
                    export PATH="$PATH:$HOME/.dotnet"
                fi
                
                dotnet --version
                '''
            }
        }

        stage('Build and Test') {
            steps {
                sh '''
                export PATH="$PATH:$HOME/.dotnet"
                dotnet restore
                dotnet build --configuration Release
                dotnet test --logger "trx;LogFileName=TestResults.trx"
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