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

        stage('Verify Tools') {
            steps {
                script {
                    // Проверка за налични инструменти
                    def hasCurl = sh(returnStatus: true, script: 'which curl') == 0
                    def hasWget = sh(returnStatus: true, script: 'which wget') == 0
                    
                    if (!hasCurl && !hasWget) {
                        error 'Трябва да имате curl или wget инсталирани'
                    }
                }
            }
        }

        stage('Setup .NET') {
            steps {
                script {
                    // Изтегляне на .NET с наличния инструмент
                    if (sh(returnStatus: true, script: 'which curl') == 0) {
                        sh '''
                        curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
                        chmod +x dotnet-install.sh
                        ./dotnet-install.sh --version $DOTNET_VERSION
                        '''
                    } else {
                        sh '''
                        wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
                        chmod +x dotnet-install.sh
                        ./dotnet-install.sh --version $DOTNET_VERSION
                        '''
                    }
                    
                    // Добавяне към PATH
                    sh '''
                    export PATH="$PATH:$HOME/.dotnet"
                    dotnet --version
                    '''
                }
            }
        }

        stage('Setup ChromeDriver') {
            steps {
                script {
                    // Изтегляне на ChromeDriver
                    if (sh(returnStatus: true, script: 'which curl') == 0) {
                        sh '''
                        curl -Lo chromedriver.zip https://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip
                        unzip -o chromedriver.zip
                        chmod +x chromedriver
                        '''
                    } else {
                        sh '''
                        wget https://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip -O chromedriver.zip
                        unzip -o chromedriver.zip
                        chmod +x chromedriver
                        '''
                    }
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