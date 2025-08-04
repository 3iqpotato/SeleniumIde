pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-v /var/run/docker.sock:/var/run/docker.sock -u root'
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
                    software-properties-common \
                    apt-utils
                '''
            }
        }

stage('Install Chrome & ChromeDriver') {
    steps {
        sh '''
        # Install Chrome
        wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
        echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list
        apt-get update
        apt-get install -y google-chrome-stable

        # Get exact Chrome version
        CHROME_FULL_VERSION=$(google-chrome --version | awk '{print $3}')
        echo "Detected Chrome version: $CHROME_FULL_VERSION"
        
        # Extract major version number
        CHROME_MAJOR_VERSION=$(echo $CHROME_FULL_VERSION | cut -d'.' -f1)
        echo "Chrome major version: $CHROME_MAJOR_VERSION"
        
        # Get ChromeDriver version with multiple fallback methods
        CHROMEDRIVER_VERSION=$(wget -qO- "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_$CHROME_MAJOR_VERSION" || \\
          curl -sS "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_$CHROME_MAJOR_VERSION" || \\
          echo "138.0.0") # Fallback version if both methods fail
        
        echo "ChromeDriver version to install: $CHROMEDRIVER_VERSION"
        
        # Download and install ChromeDriver
        wget -q "https://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip"
        unzip chromedriver_linux64.zip -d /usr/local/bin/
        chmod +x /usr/local/bin/chromedriver
        rm chromedriver_linux64.zip

        # Verify installations
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