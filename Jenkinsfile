pipeline {
    agent any

    stages {
        stage('Setup Environment') {
            steps {
                sh '''
                apt-get update
                apt-get install -y wget gnupg unzip
                '''
            }
        }

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Setup Chrome and ChromeDriver') {
            steps {
                sh '''
                # Install Chrome
                wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
                echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list
                apt-get update
                apt-get install -y google-chrome-stable

                # Install ChromeDriver
                CHROME_VERSION=$(google-chrome --version | awk '{print $3}')
                CHROMEDRIVER_VERSION=$(curl -sS https://chromedriver.storage.googleapis.com/LATEST_RELEASE_${CHROME_VERSION%.*})
                curl -Lo chromedriver.zip https://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip
                unzip -o chromedriver.zip
                chmod +x chromedriver
                mv chromedriver /usr/local/bin/
                '''
            }
        }

        stage('Build and Test') {
            steps {
                sh '''
                export PATH="$PATH:/usr/local/bin"
                dotnet restore
                dotnet build --configuration Release
                dotnet test --logger "trx;LogFileName=TestResults.trx" --results-directory ./TestResults
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