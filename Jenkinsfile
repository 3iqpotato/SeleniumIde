pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-v /var/run/docker.sock:/var/run/docker.sock -u root'
        }
    }

    environment {
        CHROME_VERSION = '120.0.6099.71'
        CHROMEDRIVER_VERSION = '120.0.6099.71'
    }

    stages {
        stage('Install Dependencies') {
            steps {
                sh '''
                apt-get update && apt-get install -y --no-install-recommends \
                    wget \
                    unzip \
                    gnupg \
                    curl \
                    ca-certificates
                '''
            }
        }

        stage('Install Chrome') {
            steps {
                sh '''
                # Add Chrome repository
                wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
                echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list
                apt-get update
                
                # Install specific Chrome version
                apt-get install -y google-chrome-stable=${CHROME_VERSION}-1
                '''
            }
        }

        stage('Install ChromeDriver') {
            steps {
                sh '''
                # Download ChromeDriver
                wget -q "https://chromedriver.storage.googleapis.com/${CHROMEDRIVER_VERSION}/chromedriver_linux64.zip"
                unzip chromedriver_linux64.zip -d /usr/local/bin/
                chmod +x /usr/local/bin/chromedriver
                rm chromedriver_linux64.zip
                '''
            }
        }

        stage('Verify') {
            steps {
                sh '''
                echo "Chrome version: $(google-chrome --version)"
                echo "ChromeDriver version: $(chromedriver --version)"
                '''
            }
        }

        stage('Build & Test') {
            steps {
                sh '''
                export CHROME_BIN=/usr/bin/google-chrome
                export CHROMEDRIVER_PATH=/usr/local/bin/chromedriver
                dotnet restore
                dotnet build
                dotnet test
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