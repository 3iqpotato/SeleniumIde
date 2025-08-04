pipeline {
    agent any

    environment {
        CHROME_VERSION = '114.0.5735.90'
        CHROMEDRIVER_VERSION = '114.0.5735.90'
    }

    stages {
        stage('Setup Environment') {
            steps {
                sh '''
                # Инсталиране на .NET 6
                wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
                dpkg -i packages-microsoft-prod.deb
                apt-get update
                apt-get install -y dotnet-sdk-6.0

                # Инсталиране на Chrome
                wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
                echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list
                apt-get update
                apt-get install -y google-chrome-stable=${env.CHROME_VERSION}-1

                # Инсталиране на ChromeDriver
                wget -N https://chromedriver.storage.googleapis.com/${env.CHROMEDRIVER_VERSION}/chromedriver_linux64.zip
                unzip chromedriver_linux64.zip
                chmod +x chromedriver
                mv chromedriver /usr/local/bin/
                '''
            }
        }

        stage('Build and Test') {
            steps {
                checkout scm
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