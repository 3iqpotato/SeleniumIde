pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Setup ChromeDriver') {
            steps {
                sh '''
                # Изтегляне на ChromeDriver без да се използва apt-get
                CHROMEDRIVER_VERSION=$(curl -sS https://chromedriver.storage.googleapis.com/LATEST_RELEASE)
                wget -O chromedriver.zip https://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip
                unzip -o chromedriver.zip
                chmod +x chromedriver
                mv chromedriver /tmp/
                '''
            }
        }

        stage('Build and Test') {
            steps {
                sh '''
                export PATH="$PATH:/tmp:$HOME/.dotnet"
                dotnet restore
                dotnet build --configuration Release
                dotnet test --logger "trx;LogFileName=TestResults.trx" --filter "FullyQualifiedName!=TC01IfUserIsInvalidTryAgainTest" --results-directory /tmp/testresults
                '''
            }
        }
    }

    post {
        always {
            junit '/tmp/testresults/*.trx'
            archiveArtifacts artifacts: '/tmp/testresults/*.trx', allowEmptyArchive: true
        }
    }
}