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
                    gnupg \  # Added gnupg package
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
                
                # Install Chrome
                apt-get update && apt-get install -y google-chrome-stable
                '''
            }
        }

        stage('Install ChromeDriver') {
            steps {
                sh '''
                # Get matching ChromeDriver version
                CHROME_VERSION=$(google-chrome --version | awk '{print $3}')
                CHROMEDRIVER_VERSION=$(curl -sS https://chromedriver.storage.googleapis.com/LATEST_RELEASE_${CHROME_VERSION%.*})
                
                # Download and install ChromeDriver
                wget -N https://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip
                unzip chromedriver_linux64.zip -d /usr/local/bin/
                chmod +x /usr/local/bin/chromedriver
                '''
            }
        }

        stage('Build & Test') {
            steps {
                sh 'dotnet restore'
                sh 'dotnet build'
                sh 'dotnet test'
            }
        }
    }
}