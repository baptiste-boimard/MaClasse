# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

ARG PROJECT_PATH=MaClasse.Client
ARG PROJECT_FILE=$PROJECT_PATH/$PROJECT_PATH.csproj

RUN dotnet restore $PROJECT_FILE
RUN dotnet publish $PROJECT_FILE -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 7235
ENTRYPOINT ["dotnet", "MaClasse.Client.dll"]

LABEL "sheperd.autodeploy" = "true"

