FROM node:10-alpine
EXPOSE 4200 49153

WORKDIR /src
COPY WeSafe.Web/ClientApp/package*.json ./
RUN npm install

COPY WeSafe.Web/ClientApp .

CMD ["npm", "run", "start:docker"]
