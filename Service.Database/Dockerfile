# Utiliser l'image .NET 9 SDK pour builder
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copier les fichiers de solution
COPY . . 

# Restaurer les packages
RUN dotnet restore

# Publier le projet (remplacer par le vrai nom du projet .csproj)
RUN dotnet publish "./Service.Database/Service.Database.csproj" -c Release -o /out --no-restore

# Image finale
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Utiliser HTTPS avec un certificat dev (aspnet core dev-certs)
EXPOSE 7012

ENTRYPOINT ["dotnet", "Service.Database.dll"]
