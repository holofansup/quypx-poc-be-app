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
                    image: 851725269187.dkr.ecr.ap-southeast-1.amazonaws.com/quypx-poc-uat-jenkins-agent:v1
                    imagePullPolicy: Always
                    workingDir: /home/jenkins/agent  
            '''
            defaultContainer 'jnlp'
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

        stage('Update Helm Manifest and Sync with ArgoCD') {
          environment {
            HELM_REPO_URL = 'https://github.com/holofansup/quypx-poc-k8s-service.git'
            HELM_REPO_BRANCH = 'main'
            HELM_REPO_CREDENTIALS_ID = 'jenkins-access-token-github'
            ARGOCD_APP_NAME = 'be-app'
            ARGOCD_REPO_URL = 'quypx-argocd.poc-h2hubgenai.com'
            ARGOCD_REPO_PATH = 'be-app'
            ARGOCD_PROJECT = 'application'
            ARGOCD_DEST_SERVER = 'https://kubernetes.default.svc'
            ARGOCD_DEST_NAMESPACE = 'be'
          }
          
          steps {
            script {
              withCredentials([usernamePassword(credentialsId: 'argocd-password', usernameVariable: 'ARGOCD_USERNAME', passwordVariable: 'ARGOCD_PASSWORD')]) {
                // Checkout the Helm manifest repository
                checkout([
                  $class: 'GitSCM',
                  branches: [[name: "*/${HELM_REPO_BRANCH}"]],
                  userRemoteConfigs: [[url: HELM_REPO_URL, credentialsId: HELM_REPO_CREDENTIALS_ID]]
                ])

              // Update the image tag in the Helm values file
                sh """
                  sed -i 's|tag:.*|tag: \"${DOCKER_TAG}\"|' ./be-app/values.yaml
                  cat ./be-app/values.yaml
                """

                // Commit and push the changes
                sh """
                  git config user.email "jenkins@holofansup.com"
                  git config user.name "Jenkins"
                  git fetch origin ${HELM_REPO_BRANCH}
                  git checkout -B ${HELM_REPO_BRANCH} || git checkout -b ${HELM_REPO_BRANCH}
                  git add ./be-app/values.yaml
                  git commit -m "Update image to ${DOCKER_TAG}" || true
                  git push origin ${HELM_REPO_BRANCH} || true
                """

                // Add the repository to ArgoCD
                sh """
                  argocd login ${ARGOCD_REPO_URL} --username ${ARGOCD_USERNAME} --password ${ARGOCD_PASSWORD} --insecure
                """

                // Check if the application exists, if not, create it
                appExists = sh(script: "argocd app get ${ARGOCD_APP_NAME}", returnStatus: true) == 0
                if (!appExists) {
                  sh """
                    argocd app create ${ARGOCD_APP_NAME} \
                      --repo ${ARGOCD_REPO_URL} \
                      --path ${ARGOCD_REPO_PATH} \
                      --dest-server ${ARGOCD_DEST_SERVER} \
                      --dest-namespace ${ARGOCD_DEST_NAMESPACE} \
                      --project ${ARGOCD_PROJECT}
                    """
                  }

                // Sync the application with ArgoCD
                sh """
                    argocd app sync ${ARGOCD_APP_NAME}
                  """
              }
            }
          }
        }
    }
}
