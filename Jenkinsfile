pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Setup Environment') {
            steps {
                sh '''
                # Инсталиране на .NET 8
                if ! command -v dotnet &> /dev/null; then
                    curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 8.0.100
                    export PATH="$PATH:$HOME/.dotnet"
                fi

                # Инсталиране на Chrome и ChromeDriver
                apt-get update
                apt-get install -y wget unzip
                wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
                echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list
                apt-get update
                apt-get install -y google-chrome-stable

                # Изтегляне на ChromeDriver
                CHROME_VERSION=$(google-chrome --version | awk '{print $3}')
                CHROMEDRIVER_VERSION=$(curl -s https://chromedriver.storage.googleapis.com/LATEST_RELEASE_${CHROME_VERSION%.*})
                wget -O chromedriver.zip https://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip
                unzip chromedriver.zip
                chmod +x chromedriver
                mv chromedriver /usr/local/bin/
                '''
            }
        }

        stage('Build and Test') {
            steps {
                sh '''
                export PATH="$PATH:$HOME/.dotnet:/usr/local/bin"
                dotnet restore
                dotnet build --configuration Release
                dotnet test --logger "trx;LogFileName=TestResults.trx"
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