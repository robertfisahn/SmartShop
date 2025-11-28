FROM node:20 AS build-env
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

RUN npm run build --configuration=production

FROM nginx:alpine
COPY --from=build-env /app/dist/smart-shop-ui/browser /usr/share/nginx/html
COPY src/assets/env.js /usr/share/nginx/html/assets/env.js
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
