FROM node:18

WORKDIR /app

COPY package.json ./
COPY booking.proto ./
RUN npm install

COPY . .

CMD ["node", "index.js"]
