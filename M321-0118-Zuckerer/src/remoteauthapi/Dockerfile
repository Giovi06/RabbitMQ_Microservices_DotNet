FROM node:20-alpine

EXPOSE 8888

ENV USERS_FILE=/data/users.json
ENV API_PORT=8888
ENV API_ADDR=0.0.0.0

WORKDIR /app

COPY users.json /data/users.json
COPY server.mjs /app/server.mjs

ENTRYPOINT [ "node", "server.mjs" ]
