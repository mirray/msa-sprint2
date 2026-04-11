***Change log - Setup PC for local development and testing with CI on staging and production***

1. booking-service-grpc - working service from task 2
2. added new entry points for health-check and feature check:
 - GET /ping
 - GET /feature 

3. wsl - installed ubuntu and set to default - to allow build under docker socket with gitlab-ci-local
4. run minikube with docker desktop
5. setup docker socket in .gitlab-ci.yml

**Docker Socket**

Pros: 
   - fast run
   - no need additional resources

Cons:
   - some unexpected behavior from time to time
   - lack of documentation or working examples

**Alternative: Docker-to-Docker**

Pros:
 - stable work
 - a lot of documentation and working examples

Cons:
 - manual initial setup or long auto-setup before each run

6. use helm chart for CI on stage/prod
7. use kompose to convert from compose.yml to easy helm charts
8. hire a DevOps engineer to maintain a positive attitude and faith in humanity within the team.
