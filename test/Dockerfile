FROM debian:bullseye-slim

WORKDIR /app

# Install dependencies
RUN apt-get update && apt-get install -y \
    curl \
    postgresql-client \
    bash \
    && rm -rf /var/lib/apt/lists/*

USER root
COPY wait-for-it.sh /usr/local/bin/wait-for-it
COPY init-fixtures.sql .
COPY init-fixtures-booking.sql .
COPY regress.sh .
RUN chmod +x /usr/local/bin/wait-for-it
RUN chmod +x regress.sh
CMD ["sh","regress.sh"]
