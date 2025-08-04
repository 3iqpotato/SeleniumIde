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
        
        stage('Setup Environment') {
            steps {
                script {
                    // Fix Chocolatey installation and PATH
                    bat '''
                    @echo off
                    setlocal enabledelayedexpansion
                    
                    :: Check if Chocolatey is properly installed
                    where choco >nul 2>&1
                    if !ERRORLEVEL! neq 0 (
                        echo Chocolatey not in PATH, checking ProgramData
                        if exist "!ALLUSERSPROFILE!\\chocolatey\\bin\\choco.exe" (
                            echo Adding Chocolatey to PATH
                            set PATH=!PATH!;!ALLUSERSPROFILE!\\chocolatey\\bin
                        ) else (
                            echo Installing Chocolatey fresh
                            powershell -NoProfile -ExecutionPolicy Bypass -Command "[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))"
                            set PATH=!PATH!;!ALLUSERSPROFILE!\\chocolatey\\bin
                        )
                    )
                    
                    :: Install Chrome
                    choco install googlechrome --version=%CHROME_VERSION% -y --allow-downgrade --ignore-checksums
                    endlocal
                    '''
                }
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
                    if not exist "%CHROME_INSTALL_PATH%" mkdir "%CHROME_INSTALL_PATH%"
                    Copy-Item -Path .\\chromedriver-win64\\chromedriver.exe -Destination "%CHROME_INSTALL_PATH%\\chromedriver.exe" -Force
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