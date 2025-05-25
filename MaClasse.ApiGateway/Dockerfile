FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

ARG BUILD_ENV=Production
RUN echo "Using environment: $BUILD_ENV"

# Build projet
ARG PROJECT_PATH=MaClasse.ApiGateway
ARG PROJECT_FILE=$PROJECT_PATH/$PROJECT_PATH.csproj

RUN dotnet restore $PROJECT_FILE
RUN dotnet publish $PROJECT_FILE -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 7261
ENTRYPOINT ["dotnet", "MaClasse.ApiGateway.dll"]

LABEL "sheperd.autodeploy" = "true"

