pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Install Prerequisites') {
            steps {
                sh '''
                # Инсталиране на curl ако липсва
                if ! command -v curl &> /dev/null; then
                    echo "Инсталиране на curl..."
                    sudo apt-get update && sudo apt-get install -y curl
                fi

                # Проверка за .NET
                if ! command -v dotnet &> /dev/null; then
                    echo "Инсталиране на .NET 6..."
                    curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 6.0.400
                    export PATH="$PATH:$HOME/.dotnet"
                fi
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