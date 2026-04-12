***Setting up with Service Mesh***


1. Use Istio over minikube, setup injections over default namespace
2. Create new version of booking-service-grpc (v1.1.0) - it reads header "X-Feature-Enabled" and return result in GET /ping
3. Extract booking-service-db in new namespace (database) to prevent injecting with Istio
4. Build and test new version of booking-service-grpc (v1.1.0)
5. Update helm chart to use version v1.1.0
6. Install new version of helm chart in new pod (setup metadata:name = release.Name,  image:tag = chart.appVersion, metadata:labels:version = chart.appVersion)
7. Setup destination rule for both services, include Retries and Circuit Breaking
8. Define virtual service to split traffic 90% and 10%
9. Define gateway to open port 80
10. Create envoy filter to check header from request and setup internal header (x-version:v2) in case when feature requested
11. Correct virtual service to check internal header and route traffic with feature to v1.1.0 service
12. Create test-istio stage for gitlab-ci to run tests from sh
