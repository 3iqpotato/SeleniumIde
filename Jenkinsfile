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
        # Get Chrome version (e.g., "138.0.7204.183")
        CHROME_VERSION=$(google-chrome --version | awk '{print $3}')
        echo "Detected Chrome version: $CHROME_VERSION"

        # Download matching ChromeDriver (new method for Chrome 115+)
        CHROMEDRIVER_DOWNLOAD_URL="https://edgedl.me.gvt1.com/edgedl/chrome/chrome-for-testing/$CHROME_VERSION/linux64/chromedriver-linux64.zip"
        echo "Downloading ChromeDriver from: $CHROMEDRIVER_DOWNLOAD_URL"
        
        wget -q "$CHROMEDRIVER_DOWNLOAD_URL" -O chromedriver-linux64.zip
        unzip chromedriver-linux64.zip -d /tmp/
        mv /tmp/chromedriver-linux64/chromedriver /usr/local/bin/
        chmod +x /usr/local/bin/chromedriver
        rm -rf chromedriver-linux64.zip /tmp/chromedriver-linux64/
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