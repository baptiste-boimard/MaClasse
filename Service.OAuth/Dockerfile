# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

ARG PROJECT_PATH=Service.OAuth
ARG PROJECT_FILE=$PROJECT_PATH/$PROJECT_PATH.csproj

RUN dotnet restore $PROJECT_FILE
RUN dotnet publish $PROJECT_FILE -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

USER root
RUN mkdir -p /app/keys && chown -R 1654:1654 /app/keys

USER app
WORKDIR /app
COPY --chown=app:app --from=build /app/out .

EXPOSE 7011
ENTRYPOINT ["dotnet", "Service.OAuth.dll"]

LABEL "sheperd.autodeploy" = "true"
