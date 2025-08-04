pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-v /var/run/docker.sock:/var/run/docker.sock -u root'  // Run as root to avoid permission issues
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
                    xdg-utils \
                    curl \
                    ca-certificates \
                    software-properties-common
                '''
            }
        }

        stage('Install Chrome & ChromeDriver') {
            steps {
                sh '''
                # Install latest Chrome
                wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
                sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list'
                apt-get update
                apt-get install -y google-chrome-stable

                # Get matching ChromeDriver
                CHROME_VERSION=$(google-chrome --version | grep -oP '\\d+\\.\\d+\\.\\d+\\.\\d+' | cut -d'.' -f1-3)
                CHROMEDRIVER_VERSION=$(wget -qO- "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_$CHROME_VERSION")
                wget "https://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip"
                unzip chromedriver_linux64.zip -d /usr/local/bin/
                chmod +x /usr/local/bin/chromedriver
                rm chromedriver_linux64.zip

                # Confirm installation
                google-chrome --version
                chromedriver --version
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