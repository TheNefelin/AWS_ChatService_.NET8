
docker build -t artema-chat-app .

docker run -d -p 5000:80 --name chat-app artema-chat-app
