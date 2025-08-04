pipeline {
    agent any

    environment {
        CHROME_VERSION = '114.0.5735.90'
        CHROMEDRIVER_VERSION = '114.0.5735.90'
    }

    stages {
        stage('Install Prerequisites') {
            steps {
                script {
                    // Проверка и инсталация на wget
                    if (!isUnix() || sh(returnStatus: true, script: 'which wget') != 0) {
                        if (isUnix()) {
                            sh 'apt-get update && apt-get install -y wget unzip'
                        } else {
                            bat 'choco install wget -y'
                        }
                    }
                }
            }
        }

        stage('Install .NET 6') {
            steps {
                script {
                    if (isUnix()) {
                        sh '''
                        wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
                        dpkg -i packages-microsoft-prod.deb
                        rm packages-microsoft-prod.deb
                        apt-get update
                        apt-get install -y dotnet-sdk-6.0
                        '''
                    } else {
                        bat 'choco install dotnet-sdk -y --version=6.0.100'
                    }
                }
            }
        }

        stage('Install Chrome') {
            steps {
                script {
                    if (isUnix()) {
                        sh '''
                        wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
                        echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list
                        apt-get update
                        apt-get install -y google-chrome-stable=${env.CHROME_VERSION}-1
                        '''
                    } else {
                        bat '''
                        choco uninstall googlechrome -y
                        choco install googlechrome --version=%CHROME_VERSION% -y --allow-downgrade
                        '''
                    }
                }
            }
        }

        stage('Install ChromeDriver') {
            steps {
                script {
                    if (isUnix()) {
                        sh '''
                        wget -N https://chromedriver.storage.googleapis.com/${env.CHROMEDRIVER_VERSION}/chromedriver_linux64.zip
                        unzip chromedriver_linux64.zip
                        chmod +x chromedriver
                        mv chromedriver /usr/local/bin/
                        '''
                    } else {
                        bat '''
                        if not exist "C:\\WebDriver" mkdir "C:\\WebDriver"
                        curl -o chromedriver_win32.zip "https://chromedriver.storage.googleapis.com/%CHROMEDRIVER_VERSION%/chromedriver_win32.zip"
                        tar -xf chromedriver_win32.zip -C "C:\\WebDriver"
                        set PATH=%PATH%;C:\\WebDriver
                        '''
                    }
                }
            }
        }

        stage('Build and Test') {
            steps {
                checkout scm
                script {
                    if (isUnix()) {
                        sh 'dotnet restore'
                        sh 'dotnet build --configuration Release'
                        sh 'dotnet test --logger "trx;LogFileName=TestResults.trx"'
                    } else {
                        bat 'dotnet restore'
                        bat 'dotnet build --configuration Release'
                        bat 'dotnet test --logger "trx;LogFileName=TestResults.trx"'
                    }
                }
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