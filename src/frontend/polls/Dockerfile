FROM node:16-alpine
WORKDIR /app
COPY . .

RUN npm i -g
RUN yarn install
RUN npm run build

ENV NODE_ENV production

EXPOSE 3000
# Start the app
CMD [ "npx", "serve", "build" ]