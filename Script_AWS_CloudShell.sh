# Login to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456.dkr.ecr.us-east-1.amazonaws.com

# Check disk space
df -h

# Clean Docker system (images, containers, cache)
docker system prune -a -f

# Clean Docker volumes (careful, deletes unused volumes)
docker volume prune -f

# Dlete file or folder if it exists
rm -rf AWS_ChatService_.NET8
rm text.txt

# Clone repo
git clone [REPO_URL]

# Enter project folder
cd AWS_ChatService_.NET8
ls -a

# Check disk space again
tar -czf context.tar.gz .dockerignore Dockerfile * && du -sh context.tar.gz

# Build Docker image
docker build -t dotnet-app-repo .
docker build --no-cache -t dotnet-app-repo .

# Tag Docker image for ECR
docker tag dotnet-app-repo:latest 123456.dkr.ecr.us-east-1.amazonaws.com/dotnet-app-repo:latest

# Push Docker image to ECR
docker push 123456.dkr.ecr.us-east-1.amazonaws.com/dotnet-app-repo:latest
