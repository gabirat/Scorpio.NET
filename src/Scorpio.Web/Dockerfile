FROM node:latest 
# TODO change to alpine image, lock version

RUN apt-get -y update
COPY . /webapp
WORKDIR /webapp
#ENV CORSHEADER "http://api:80"
CMD npm install && HOST=0.0.0.0 npm start