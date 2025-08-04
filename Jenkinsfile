pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-u root -v /var/run/docker.sock:/var/run/docker.sock'
        }
    }

    stages {
        stage('Checkout Code') {
            steps {
                checkout scm
            }
        }

        stage('Install Dependencies') {
            steps {
                sh '''
                apt-get update && apt-get install -y --no-install-recommends \
                    wget \
                    unzip \
                    gnupg \
                    xvfb \
                    libxi6 \
                    libgconf-2-4 \
                    fonts-liberation \
                    libappindicator1 \
                    libnss3 \
                    lsb-release \
                    xdg-utils
                '''
            }
        }

        stage('Install Chrome') {
            steps {
                sh '''
                # Add Google Chrome repository
                wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | gpg --dearmor -o /usr/share/keyrings/googlechrome-linux-keyring.gpg
                echo "deb [arch=amd64 signed-by=/usr/share/keyrings/googlechrome-linux-keyring.gpg] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list
                
                # Install specific Chrome version (114.x which is more stable)
                apt-get update && apt-get install -y google-chrome-stable=114.0.5735.198-1
                '''
            }
        }

        stage('Install ChromeDriver') {
            steps {
                sh '''
                # Get matching ChromeDriver version for Chrome 114
                CHROMEDRIVER_VERSION="114.0.5735.90"
                wget -N https://chromedriver.storage.googleapis.com/${CHROMEDRIVER_VERSION}/chromedriver_linux64.zip
                unzip chromedriver_linux64.zip -d /usr/local/bin/
                chmod +x /usr/local/bin/chromedriver
                rm chromedriver_linux64.zip
                '''
            }
        }

        stage('Build & Test') {
            steps {
                sh 'dotnet restore'
                sh 'dotnet build'
                sh '''
                # Run tests with Chrome binary location
                export CHROME_PATH=/usr/bin/google-chrome-stable
                export CHROMEDRIVER_PATH=/usr/local/bin/chromedriver
                dotnet test
                '''
            }
        }
    }
}