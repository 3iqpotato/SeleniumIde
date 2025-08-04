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
                    
                    :: Force Chrome installation
                    choco install googlechrome --version=%CHROME_VERSION% -y --force --ignore-checksums
                    endlocal
                    '''
                }
            }
        }
        
        stage('Setup ChromeDriver') {
            steps {
                script {
                    writeFile file: 'download_chromedriver.ps1', text: '''
                    $ProgressPreference = 'SilentlyContinue'
                    $url = "https://edgedl.me.gvt1.com/edgedl/chrome/chrome-for-testing/${env:CHROMEDRIVER_VERSION}/win64/chromedriver-win64.zip"
                    $downloadPath = "$env:WORKSPACE\\chromedriver.zip"
                    $extractPath = "$env:WORKSPACE"
                    $destinationPath = "${env:CHROME_INSTALL_PATH}\\chromedriver.exe"
                    
                    try {
                        Invoke-WebRequest -Uri $url -OutFile $downloadPath -UseBasicParsing
                        Expand-Archive -Path $downloadPath -DestinationPath $extractPath -Force
                        if (!(Test-Path $env:CHROME_INSTALL_PATH)) {
                            New-Item -ItemType Directory -Path $env:CHROME_INSTALL_PATH -Force
                        }
                        Copy-Item -Path "$extractPath\\chromedriver-win64\\chromedriver.exe" -Destination $destinationPath -Force
                        exit 0
                    } catch {
                        Write-Error $_
                        exit 1
                    }
                    '''
                    
                    bat '''
                    @echo off
                    echo Downloading ChromeDriver %CHROMEDRIVER_VERSION%
                    powershell -ExecutionPolicy Bypass -File "%WORKSPACE%\\download_chromedriver.ps1"
                    '''
                }
            }
        }
        
        stage('Build and Test') {
            steps {
                bat '''
                dotnet restore SeleniumIde.sln
                dotnet build SeleniumIde.sln --configuration Release
                if not exist TestResults mkdir TestResults
                dotnet test SeleniumIde.sln --logger "trx;LogFileName=TestResults\\TestResults.trx"
                dotnet tool install -g trx2junit
                trx2junit TestResults\\TestResults.trx
                '''
            }
        }
    }
    
    post {
        always {
            archiveArtifacts artifacts: '**/TestResults/*.*', allowEmptyArchive: true
            junit '**/TestResults/*.xml'  // Търси JUnit XML файлове, които се генерират от trx2junit
        }
    }
}