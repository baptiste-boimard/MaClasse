# Utiliser l'image .NET 9 SDK pour builder
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copier les fichiers de solution
COPY . . 

# Restaurer les packages
RUN dotnet restore

# Publier le projet (remplacer par le vrai nom du projet .csproj)
RUN dotnet publish "./MaClasse.ApiGateway/MaClasse.ApiGateway.csproj" -c Release -o /out --no-restore

# Image finale
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# 👇 Ajouter cette ligne pour s'assurer que le fichier est bien là
COPY ./MaClasse.ApiGateway/ocelot.json .

# Utiliser HTTPS avec un certificat dev (aspnet core dev-certs)
EXPOSE 7261

ENTRYPOINT ["dotnet", "MaClasse.ApiGateway.dll"]
