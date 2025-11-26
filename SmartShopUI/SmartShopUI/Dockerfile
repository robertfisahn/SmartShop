FROM node:20 AS build-env
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

ARG API_URL
ENV API_URL=${API_URL:-http://localhost:5888}

RUN sed -i "s|__API_URL__|${API_URL}|g" src/environments/environment.prod.ts

RUN npm run build --configuration=production

FROM nginx:alpine
COPY --from=build-env /app/dist/smart-shop-ui/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
