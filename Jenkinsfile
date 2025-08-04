pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:6.0'
            args '--user root'  // За административни права
        }
    }

    environment {
        CHROME_VERSION = '114.0.5735.90'
        CHROMEDRIVER_VERSION = '114.0.5735.90'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Install Chrome') {
            steps {
                sh '''
                wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
                echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list
                apt-get update
                apt-get install -y google-chrome-stable=${env.CHROME_VERSION}-1
                '''
            }
        }

        stage('Install ChromeDriver') {
            steps {
                sh '''
                wget -N https://chromedriver.storage.googleapis.com/${env.CHROMEDRIVER_VERSION}/chromedriver_linux64.zip
                unzip chromedriver_linux64.zip
                chmod +x chromedriver
                mv chromedriver /usr/local/bin/
                '''
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
            archiveArtifacts artifacts: '**/TestResults/*.trx', allowEmptyArchive: true
            junit '**/TestResults/*.trx'
        }
    }
}