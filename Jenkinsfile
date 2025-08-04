pipeline {
    agent any
    
    environment {
        CHROME_VERSION = '127.0.6533.73'
        CHROMEDRIVER_VERSION = '127.0.6533.72'
        CHROME_INSTALL_PATH = 'C:\\Program Files\\Google\\Chrome\\Application'
        CHROMEDRIVER_PATH = 'C:\\Program Files\\Google\\Chrome\\Application\\chromedriver.exe'
    }
    
    stages {
        stage('Checkout code') {
            steps {
                git branch: 'main', 
                url: 'https://github.com/3iqpotato/SeleniumIde.git'
            }
        }
        
        stage('Set up environment') {
            steps {
                script {
                    // Install Chocolatey if not present
                    bat '''
                    @echo off
                    where choco >nul 2>&1
                    if %ERRORLEVEL% neq 0 (
                        echo Installing Chocolatey
                        powershell -NoProfile -ExecutionPolicy Bypass -Command "[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))"
                        set PATH=%PATH%;%ALLUSERSPROFILE%\\chocolatey\\bin
                    ) else (
                        echo Chocolatey already installed
                    )
                    '''
                    
                    // Refresh environment to make choco available
                    bat 'refreshenv'
                    
                    // Install .NET SDK 6.0
                    bat 'choco install dotnet-sdk -y --version=6.0.100'
                    
                    // Uninstall current Chrome
                    bat 'choco uninstall googlechrome -y'
                    
                    // Install specific Chrome version
                    bat "choco install googlechrome --version=%CHROME_VERSION% -y --allow-downgrade --ignore-checksums"
                }
            }
        }
        
        stage('Set up ChromeDriver') {
            steps {
                bat '''
                echo Downloading ChromeDriver version %CHROMEDRIVER_VERSION%
                powershell -command "Invoke-WebRequest -Uri https://chromedriver.storage.googleapis.com/%CHROMEDRIVER_VERSION%/chromedriver_win32.zip -OutFile chromedriver.zip -UseBasicParsing"
                powershell -command "Expand-Archive -Path chromedriver.zip -DestinationPath . -Force"
                if not exist "%CHROME_INSTALL_PATH%" mkdir "%CHROME_INSTALL_PATH%"
                powershell -command "Move-Item -Path .\\chromedriver.exe -Destination '%CHROMEDRIVER_PATH%' -Force"
                '''
            }
        }
        
        stage('Restore dependencies') {
            steps {
                bat 'dotnet restore SeleniumIde.sln'
            }
        }
        
        stage('Build') {
            steps {
                bat 'dotnet build SeleniumIde.sln --configuration Release'
            }
        }
        
        stage('Run tests') {
            steps {
                bat 'dotnet test SeleniumIde.sln --logger "trx;LogFileName=TestResults.trx"'
            }
        }
    }
    
    post {
        always {
            archiveArtifacts artifacts: '**/TestResults/*.trx', allowEmptyArchive: true
            junit '**/TestResults/*.trx'
        }
    }
}