pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Setup Chrome and ChromeDriver') {
            steps {
                sh '''
                # Инсталиране на Google Chrome
                curl -sS -o /tmp/google-chrome-stable_current_amd64.deb https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
                sudo dpkg -i /tmp/google-chrome-stable_current_amd64.deb || sudo apt-get install -fy
                
                # Инсталиране на ChromeDriver
                CHROME_VERSION=$(google-chrome --version | awk '{print $3}')
                CHROMEDRIVER_VERSION=$(curl -sS https://chromedriver.storage.googleapis.com/LATEST_RELEASE_${CHROME_VERSION%.*})
                curl -Lo /tmp/chromedriver.zip https://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip
                unzip -o /tmp/chromedriver.zip -d /tmp/
                chmod +x /tmp/chromedriver
                sudo mv /tmp/chromedriver /usr/local/bin/
                '''
            }
        }

        stage('Build and Test') {
            steps {
                sh '''
                export PATH="$PATH:/usr/local/bin:$HOME/.dotnet"
                dotnet restore
                dotnet build --configuration Release
                dotnet test --logger "trx;LogFileName=TestResults.trx" --results-directory /tmp/testresults
                '''
            }
        }
    }

    post {
        always {
            junit '/tmp/testresults/*.trx'
            archiveArtifacts artifacts: '/tmp/testresults/*.trx', allowEmptyArchive: true
        }
    }
}