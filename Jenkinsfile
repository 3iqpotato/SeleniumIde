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
                
                # Install latest stable Chrome
                apt-get update && apt-get install -y google-chrome-stable
                
                # Verify installation
                google-chrome --version
                '''
            }
        }

        stage('Install ChromeDriver') {
            steps {
                sh '''
                # Get installed Chrome version
                CHROME_VERSION=$(google-chrome --version | awk '{print $3}')
                echo "Installed Chrome version: $CHROME_VERSION"
                
                # Get major version number
                MAJOR_VERSION=$(echo $CHROME_VERSION | cut -d'.' -f1)
                echo "Major version: $MAJOR_VERSION"
                
                # Download matching ChromeDriver
                CHROMEDRIVER_VERSION=$(wget -q -O - "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_$MAJOR_VERSION")
                echo "Matching ChromeDriver version: $CHROMEDRIVER_VERSION"
                
                wget -N https://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip
                unzip -o chromedriver_linux64.zip -d /usr/local/bin/
                chmod +x /usr/local/bin/chromedriver
                rm chromedriver_linux64.zip
                
                # Verify ChromeDriver
                chromedriver --version
                '''
            }
        }

        stage('Build & Test') {
            steps {
                sh '''
                export CHROME_BIN=/usr/bin/google-chrome-stable
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