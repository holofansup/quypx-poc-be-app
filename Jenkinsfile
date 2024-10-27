pipeline {
    agent {
        kubernetes {
          label 'jenkins-agent'
        }
    }

    options {
      timeout(time: 2, unit: 'HOURS')
      disableResume()
      disableConcurrentBuilds()
      buildDiscarder(
        logRotator(numToKeepStr: '15')
      )
    }


    stages {
        stage('Set Environment Variables') {
          steps {
            script {
                
                DOCKER_TAG = "v1"

                AWS_ACCOUNT_ID = '851725269187'
                AWS_REGION = 'ap-southeast-1'
                IMAGE_NAME = 'quypx-poc-uat-be-app'
                AWS_ECR_URI = "${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
            }
          }
        }

        // stage('Checkout') {
        //   environment {
        //     REPO_URL = 'https://github.com/quypx/quypx-poc-be-app.git'
        //     BRANCH = 'main'
        //     CREDENTIALS_ID = 'jenkins-access-token-github'
        //   }
          
        //   steps {
        //     checkout([
        //       $class: 'GitSCM',
        //       branches: [[name: "*/${BRANCH}"]],
        //       userRemoteConfigs: [[url: REPO_URL, credentialsId: CREDENTIALS_ID]]
        //     ])
        //   }
        // }

        stage('Build Docker Image') {
          environment {
            Dockerfile = './cirrus/Dockerfile'
            DESTINATION = "${AWS_ECR_URI}/${IMAGE_NAME}:${DOCKER_TAG}"
          }
          
          steps {
            container('kaniko') {
              script {
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
}