pipeline {
    agent any

    environment {
        CHROME_VERSION = '114.0.5735.90'
        CHROMEDRIVER_VERSION = '114.0.5735.90'
    }

    stages {
        stage('Install Prerequisites') {
            steps {
                sh '''
                # Проверка за root права
                if [ "$(id -u)" -ne 0 ]; then
                    echo "Изпълняване с sudo права"
                    sudo apt-get update || echo "Неуспешно обновяване на пакетите"
                    sudo apt-get install -y wget unzip apt-transport-https || echo "Неуспешна инсталация на зависимости"
                else
                    apt-get update
                    apt-get install -y wget unzip apt-transport-https
                fi
                '''
            }
        }

        stage('Install .NET 6') {
            steps {
                sh '''
                # Инсталиране на .NET 6
                wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
                sudo dpkg -i packages-microsoft-prod.deb
                rm packages-microsoft-prod.deb
                sudo apt-get update
                sudo apt-get install -y dotnet-sdk-6.0
                '''
            }
        }

        stage('Install Chrome') {
            steps {
                sh '''
                # Инсталиране на Chrome
                wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | sudo apt-key add -
                echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" | sudo tee /etc/apt/sources.list.d/google.list
                sudo apt-get update
                sudo apt-get install -y google-chrome-stable=${env.CHROME_VERSION}-1
                '''
            }
        }

        stage('Install ChromeDriver') {
            steps {
                sh '''
                # Инсталиране на ChromeDriver
                wget -N https://chromedriver.storage.googleapis.com/${env.CHROMEDRIVER_VERSION}/chromedriver_linux64.zip
                unzip chromedriver_linux64.zip
                chmod +x chromedriver
                sudo mv chromedriver /usr/local/bin/
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