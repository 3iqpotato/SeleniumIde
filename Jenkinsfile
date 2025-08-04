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

        stage('Install Specific Chrome Version') {
            steps {
                sh '''
                # Install Chrome 115 (known stable version)
                wget https://dl.google.com/linux/chrome/deb/pool/main/g/google-chrome-stable/google-chrome-stable_115.0.5790.170-1_amd64.deb
                dpkg -i google-chrome-stable_115.0.5790.170-1_amd64.deb || apt-get -f install -y
                rm google-chrome-stable_115.0.5790.170-1_amd64.deb
                '''
            }
        }

        stage('Install Matching ChromeDriver') {
            steps {
                sh '''
                # Install ChromeDriver 115.0.5790.170
                wget https://chromedriver.storage.googleapis.com/115.0.5790.170/chromedriver_linux64.zip
                unzip chromedriver_linux64.zip -d /usr/local/bin/
                chmod +x /usr/local/bin/chromedriver
                rm chromedriver_linux64.zip
                
                # Verify versions
                echo "Chrome version: $(google-chrome --version)"
                echo "ChromeDriver version: $(chromedriver --version)"
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