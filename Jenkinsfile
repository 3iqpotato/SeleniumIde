pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-v /var/run/docker.sock:/var/run/docker.sock -v $WORKSPACE/.cache:/home/seluser/.cache'
            user 'root'  // ??? ТОВА Е НОВИЯТ РЕД
        }
    }


    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = "1"
        DOTNET_CLI_HOME = "${WORKSPACE}"
        XDG_CACHE_HOME = "${WORKSPACE}/.cache"
    }

    stages {
        stage('Checkout Code') {
            steps {
                git branch: 'main', url: 'https://github.com/3iqpotato/SeleniumIde.git'
            }
        }

        stage('Restore Dependencies') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build --no-restore'
            }
        }

        stage('Setup Chrome & Chromedriver') {
            steps {
                sh '''
                apt-get update
                apt-get install -y wget unzip xvfb libxi6 libgconf-2-4

                # Инсталиране на Google Chrome stable
                wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
                dpkg -i google-chrome-stable_current_amd64.deb || apt-get -f install -y

                # Изтегляне и инсталиране на ChromeDriver (примерно за версия 116)
                CHROME_VERSION=$(google-chrome --version | grep -oP '\\d+\\.\\d+\\.\\d+')
                wget -N https://chromedriver.storage.googleapis.com/116.0.5845.96/chromedriver_linux64.zip
                unzip chromedriver_linux64.zip -d /usr/local/bin/
                chmod +x /usr/local/bin/chromedriver
                '''
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test --no-build'
            }
        }

        stage('Prepare for E2E Tests') {
            steps {
                echo '?? Тук ще пуснем Selenium + Chrome (E2E тестове)'
                // Добави твоята логика за E2E тестове
            }
        }
    }
}
