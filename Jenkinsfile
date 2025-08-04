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
                    apt-transport-https \
                    software-properties-common \
                    libxss1 \
                    libxtst6 \
                    libx11-xcb1 \
                    libxcomposite1 \
                    libxcursor1 \
                    libxdamage1 \
                    libxi6 \
                    libxtst6 \
                    libnss3 \
                    libcups2 \
                    libxrandr2 \
                    libasound2 \
                    libatk1.0-0 \
                    libatk-bridge2.0-0 \
                    libpangocairo-1.0-0 \
                    libgtk-3-0
                '''
            }
        }

        stage('Install Chrome') {
            steps {
                sh '''
                # Install Chrome using the direct download method (more reliable)
                wget -q https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
                apt-get install -y ./google-chrome-stable_current_amd64.deb
                rm google-chrome-stable_current_amd64.deb
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