pipeline {
    agent any

    environment {
        CHROME_VERSION = '127.0.6533.73'
        CHROMEDRIVER_VERSION = '127.0.6533.72'
        // Променете пътищата според вашия Docker контейнер
        CHROME_INSTALL_PATH = '/usr/bin/google-chrome'  // Linux път в Docker
        CHROMEDRIVER_PATH = '/usr/local/bin/chromedriver'
    }

    stages {
        stage('Checkout code') {
            steps {
                git branch: 'main', 
                url: 'https://github.com/<your-repo>/SeleniumIDE.git'
            }
        }

        stage('Set up .NET Core') {
            steps {
                sh 'dotnet --list-sdks || echo ".NET not installed"'
                // За Docker, по-добре е да използвате предварително конфигуриран image с .NET
                sh 'dotnet restore SeleniumIde.sln'
            }
        }

        stage('Chrome Setup') {
            steps {
                sh '''
                # Инсталиране на Chrome
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
                # Сваляне и инсталиране на ChromeDriver
                wget -N https://chromedriver.storage.googleapis.com/${env.CHROMEDRIVER_VERSION}/chromedriver_linux64.zip
                unzip chromedriver_linux64.zip
                chmod +x chromedriver
                mv chromedriver ${env.CHROMEDRIVER_PATH}
                '''
            }
        }

        stage('Build and Test') {
            steps {
                sh 'dotnet build SeleniumIde.sln --configuration Release'
                sh 'dotnet test SeleniumIde.sln --logger "trx;LogFileName=TestResults.trx"'
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