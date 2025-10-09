pipeline {
    agent any
    
    environment {
        CHROME_VERSION = '127.0.6533.73'
        CHROMEDRIVER_VERSION = '127.0.6533.73'
        CHROME_INSTALL_PATH = 'C:\\Program Files\\Google\\Chrome\\Application'
    }
    
    stages {
        stage('Checkout code') {
            steps {
                git branch: 'main', 
                url: 'https://github.com/3iqpotato/SeleniumIde.git'
            }
        }
        
        
        
stage('Build and Test') {
    steps {
        bat '''
        dotnet restore SeleniumIde.sln
        dotnet build SeleniumIde.sln --configuration Release
        if not exist TestResults mkdir TestResults
        dotnet test SeleniumIde.sln --logger "trx;LogFileName=TestResults\\TestResults.trx"
        '''
    }
}

    }
    
}
