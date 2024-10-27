pipeline {
    agent {
        kubernetes {
          label 'jenkins-agent'
        }
    }

    options {
      skipDefaultCheckout()
      timeout(time: 1, unit: 'HOURS')
      disableResume()
      disableConcurrentBuilds()
      buildDiscarder(
        logRotator(numToKeepStr: '15')
      )
      timestamps()
    }


    stages {
        stage('Set Environment Variables') {
          environment {
            COMMIT_ID = sh(returnStdout: true, script: "git rev-parse --short HEAD").trim()
            GIT_TAG = sh(returnStdout: true, script: "git describe --exact-match --abbrev=0 || echo NONE").trim()
            DOCKER_TAG = "${COMMIT_ID}"

            AWS_ACCOUNT_ID = '851725269187'
            AWS_REGION = 'ap-southeast-1'
            IMAGE_NAME = 'quypx-poc-uat-be-app'
            AWS_ECR_URI = "${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
          }
        }

        stage('Checkout') {
          steps {
            def repo_url = 'https://github.com/quypx/quypx-poc-be-app.git'
            def branch = 'main'
            def credentials_id = 'jenkins-access-token-github'
            
            checkout([
              $class: 'GitSCM',
              branches: [[name: "*/${branch}"]],
              userRemoteConfigs: [[url: repo_url, credentialsId: credentials_id]]
            ])
          }
        }

        stage('Build Docker Image') {
          agent {
            label 'jenkins-agent'
          }

          environment {
            Dockerfile = './cirrus/Dockerfile'
            DESTINATION = "${AWS_ECR_URI}/${IMAGE_NAME}:${DOCKER_TAG}"
          }
          
          steps {
            container('kaniko', shell: '/busybox/sh') {
              sh """#!/busybox/sh
                /kaniko/executor \
                --context=. \
                --dockerfile=${Dockerfile} \
                --destination ${DESTINATION}
              """
            }
          }
        }
    }
}