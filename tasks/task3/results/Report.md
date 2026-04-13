Changes:

Booking subgraph:
1. Grpc request to Booking Service 
2. ACL - requests to list of booking without correct header "userid" will return empty array []
3. hotel requested by typename and id

Hotel subgraph:
1. Rest request to Monolith API
2. DataLoader + resolveReference to avoid N+1 problem

Promocode subgraph:
1. Mock new discounts functionality
2. Extend booking with new discountInfo type
3. calculate discount based on data from booking
