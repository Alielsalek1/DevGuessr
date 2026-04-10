FROM node:22-alpine AS build
WORKDIR /src

COPY Frontend/package*.json ./Frontend/
RUN cd Frontend && npm ci

COPY Frontend/ ./Frontend/
RUN cd Frontend && npm run build -- --configuration production

FROM nginx:1.27-alpine AS runtime
RUN apk add --no-cache gettext
COPY Nginx/frontend/default.conf /etc/nginx/conf.d/default.conf
COPY --from=build /src/Frontend/dist/frontend/browser/ /usr/share/nginx/html/
COPY Frontend/docker-entrypoint.d/ /docker-entrypoint.d/
RUN chmod +x /docker-entrypoint.d/*.sh

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
