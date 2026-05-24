FROM node:20-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build

FROM nginx:1.27-alpine AS runtime
ARG API_UPSTREAM=http://api:8080
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
RUN sed -i "s|API_UPSTREAM_PLACEHOLDER|${API_UPSTREAM}|g" /etc/nginx/conf.d/default.conf
EXPOSE 80
