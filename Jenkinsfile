pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-v /var/run/docker.sock:/var/run/docker.sock'
        }
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = "1"
    }

    stages {
        stage('Checkout Code') {
            steps {
                git branch: 'main', url: 'https://github.com/3iqpotato/SeleniumIde.git'
            }
        }

        stage('Restore Dependencies') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build --no-restore'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test --no-build'
            }
        }

        stage('Prepare for E2E Tests') {
            steps {
                echo '?? Тук ще пуснем Selenium + Chrome'
                // Можеш да добавиш допълнителен docker image тук
            }
        }
    }
}
