# Login to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456.dkr.ecr.us-east-1.amazonaws.com

# Check disk space
df -h

# Clean Docker system (images, containers, cache)
docker system prune -a -f

# Clean Docker volumes (careful, deletes unused volumes)
docker volume prune -f

# Clone repo
git clone [REPO_URL]

# Enter project folder
cd [REPO_NAME]

# Build Docker image
docker build -t dotnet-app-repo .

# Tag Docker image for ECR
docker tag dotnet-app-repo:latest 123456.dkr.ecr.us-east-1.amazonaws.com/dotnet-app-repo:latest

# Push Docker image to ECR
docker push 123456.dkr.ecr.us-east-1.amazonaws.com/dotnet-app-repo:latest
