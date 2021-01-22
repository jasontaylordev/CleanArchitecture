FROM node:12
EXPOSE 4200
WORKDIR /usr/src/app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm build
ENTRYPOINT ["npm", "run", "start:dev"]
