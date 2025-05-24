# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier tout le repo
COPY . .

# Construire uniquement le projet Service.OAuth
ARG PROJECT_PATH=Service.OAuth
WORKDIR /src/$PROJECT_PATH

RUN dotnet restore
RUN dotnet publish -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 7011
ENTRYPOINT ["dotnet", "Service.OAuth.dll"]
