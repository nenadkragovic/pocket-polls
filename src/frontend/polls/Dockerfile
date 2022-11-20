FROM node:16-alpine as build
WORKDIR /app
ENV PATH /app/node_modules/.bin:$PATH
COPY . .

RUN npm i -g
RUN yarn install
RUN npm run build

FROM nginx:alpine as server

WORKDIR /home

COPY --from=build /app .

COPY nginx.conf /etc/nginx/

RUN cp -a build/* /usr/share/ngnix/html

EXPOSE 3000

ENTRYPOINT ["nginx"]