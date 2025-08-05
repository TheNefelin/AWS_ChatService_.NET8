# Fase de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copiar solución y archivos .csproj individuales (para caché eficiente)
COPY ["AWS_ChatService_.NET8.sln", "."]
COPY ["AWS_ChatService_API/AWS_ChatService_API.csproj", "AWS_ChatService_API/"]
COPY ["AWS_ChatService_Application/AWS_ChatService_Application.csproj", "AWS_ChatService_Application/"]
COPY ["AWS_ChatService_Domain/AWS_ChatService_Domain.csproj", "AWS_ChatService_Domain/"]
COPY ["AWS_ChatService_Infrastructure/AWS_ChatService_Infrastructure.csproj", "AWS_ChatService_Infrastructure/"]

# 2. Restaurar dependencias
RUN dotnet restore "AWS_ChatService_API/AWS_ChatService_API.csproj"

# 3. Copiar TODO el código fuente
COPY . .

# 4. Construir el proyecto principal
WORKDIR "/src/AWS_ChatService_API"
RUN dotnet build "AWS_ChatService_API.csproj" -c Release -o /app/build

# Fase de publicación
FROM build AS publish
RUN dotnet publish "AWS_ChatService_API.csproj" -c Release -o /app/publish

# Fase final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configuraciones clave para resolver los problemas:
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "AWS_ChatService_API.dll"]