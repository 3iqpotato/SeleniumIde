pipeline {
    agent any
    
    environment {
        CHROME_VERSION = '127.0.6533.73'
        CHROMEDRIVER_VERSION = '127.0.6533.73' // Must match Chrome version exactly
        CHROME_INSTALL_PATH = 'C:\\Program Files\\Google\\Chrome\\Application'
    }
    
    stages {
        stage('Checkout code') {
            steps {
                git branch: 'main', 
                url: 'https://github.com/3iqpotato/SeleniumIde.git'
            }
        }
        
        stage('Install Chrome') {
            steps {
                bat '''
                @echo off
                where choco >nul 2>&1
                if %ERRORLEVEL% neq 0 (
                    echo Installing Chocolatey
                    powershell -NoProfile -ExecutionPolicy Bypass -Command "[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))"
                )
                call choco install googlechrome --version=%CHROME_VERSION% -y --allow-downgrade --ignore-checksums
                '''
            }
        }
        
        stage('Setup ChromeDriver') {
            steps {
                bat '''
                @echo off
                echo Downloading ChromeDriver %CHROMEDRIVER_VERSION%
                powershell -Command "& {
                    $ProgressPreference = 'SilentlyContinue'
                    $url = 'https://edgedl.me.gvt1.com/edgedl/chrome/chrome-for-testing/%CHROMEDRIVER_VERSION%/win64/chromedriver-win64.zip'
                    Invoke-WebRequest -Uri $url -OutFile chromedriver.zip -UseBasicParsing
                    Expand-Archive -Path chromedriver.zip -DestinationPath . -Force
                    Copy-Item -Path .\\chromedriver-win64\\chromedriver.exe -Destination '%CHROME_INSTALL_PATH%\\chromedriver.exe' -Force
                }"
                '''
            }
        }
        
        stage('Build and Test') {
            steps {
                bat '''
                dotnet restore SeleniumIde.sln
                dotnet build SeleniumIde.sln --configuration Release
                dotnet test SeleniumIde.sln --logger "trx;LogFileName=TestResults.trx"
                '''
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