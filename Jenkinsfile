pipeline {
    agent {
      kubernetes {
        yaml '''
          apiVersion: v1
          kind: Pod
          spec:
            containers:
              - name: kaniko
                image: gcr.io/kaniko-project/executor:debug
                workingDir: /home/jenkins/agent
                imagePullPolicy: Always
                command:
                  - /busybox/cat
                tty: true
              - name: jnlp
                image: jenkins/inbound-agent:alpine
                imagePullPolicy: Always
                workingDir: /home/jenkins/agent  
        '''
        defaultContainer: 'jnlp'
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

        stage('Checkout') {
          environment {
            REPO_URL = 'https://github.com/holofansup/quypx-poc-be-app.git'
            BRANCH = 'main'
            CREDENTIALS_ID = 'jenkins-access-token-github'
          }
          
          steps {
            checkout([
              $class: 'GitSCM',
              branches: [[name: "*/${BRANCH}"]],
              userRemoteConfigs: [[url: REPO_URL, credentialsId: CREDENTIALS_ID]]
            ])
          }
        }

        stage('Build Docker Image') {
          environment {
            Dockerfile = '`pwd`/cirrus/Dockerfile'
            DESTINATION = "${AWS_ECR_URI}/${IMAGE_NAME}:${DOCKER_TAG}"
          }
          
          steps {
            container('kaniko') {
              script {
                sh """
                  /kaniko/executor \
                  --context=`pwd` \
                  --dockerfile=${Dockerfile} \
                  --destination ${DESTINATION}
                """
              }
            }
          }
        }
    }
}