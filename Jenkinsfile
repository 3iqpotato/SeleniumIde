pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-v /var/run/docker.sock:/var/run/docker.sock -u root'
        }
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
                    ca-certificates \
                    apt-utils
                '''
            }
        }

        stage('Install Chrome') {
            steps {
                sh '''
                # Add Chrome repository (modern method without apt-key)
                mkdir -p /etc/apt/keyrings
                wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub > /etc/apt/keyrings/google-chrome.gpg
                echo "deb [arch=amd64 signed-by=/etc/apt/keyrings/google-chrome.gpg] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list
                apt-get update
                
                # Install latest stable Chrome
                apt-get install -y google-chrome-stable
                '''
            }
        }

        stage('Install ChromeDriver') {
            steps {
                sh '''
                # Get Chrome major version
                CHROME_MAJOR=$(google-chrome --version | awk '{print $3}' | cut -d'.' -f1)
                echo "Detected Chrome major version: $CHROME_MAJOR"
                
                # Get matching ChromeDriver version
                CHROMEDRIVER_VERSION=$(wget -qO- "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_$CHROME_MAJOR")
                echo "Downloading ChromeDriver version: $CHROMEDRIVER_VERSION"
                
                # Download and install
                wget -q "https://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip"
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