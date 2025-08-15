
### Opcion 1 ######################################################

# Login to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456.dkr.ecr.us-east-1.amazonaws.com

# Check disk space
df -h

# Clean Docker system (images, containers, cache)
docker system prune -a -f

# Clean Docker volumes (careful, deletes unused volumes)
docker volume prune -f

# Clean Docker system including volumes
docker system prune -a --volumes -f

# Dlete file or folder if it exists
rm -rf AWS_ChatService_.NET8
rm text.txt

# Clone repo
git clone https://github.com/TheNefelin/AWS_ChatService_.NET8.git

# Enter project folder
ls
pwd
cd /AWS_ChatService_.NET8
ls -a

# Check disk space again
tar -czf context.tar.gz .dockerignore Dockerfile * && du -sh context.tar.gz
tar --exclude-from=.dockerignore -cf - . | wc -c

# Build Docker image
docker build -t dotnet-app-repo .

# Tag Docker image for ECR
docker tag dotnet-app-repo:latest 123456.dkr.ecr.us-east-1.amazonaws.com/dotnet-app-repo:latest

# Push Docker image to ECR
docker push 123456.dkr.ecr.us-east-1.amazonaws.com/dotnet-app-repo:latest

### Opcion 2 ######################################################

# 1. Hacer un build ..bin\Release\net8.0\publish
# 2. Crear Dockerfile liviano y agregarlo en ..bin\Release\net8.0\publish
# 3. Subir publish.zip a AWS CloudShell
# 4. Descomprimir 
unzip publish.zip
# 5. Crear imagen Docker y subirla a ECR 
docker images

### Dockerfile ####################################################

# Etapa final y única (solo runtime, no SDK)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Establecer el directorio de trabajo
WORKDIR /app

# Copiar la aplicación publicada desde el contexto local
COPY . .

# Configurar el entorno (opcional)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Exponer el puerto
EXPOSE 80

# Iniciar la aplicación
ENTRYPOINT ["dotnet", "AWS_ChatService_API.dll"]

###################################################################
