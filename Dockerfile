FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /opt/app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY src/ ./src/
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime

LABEL org.opencontainers.image.title="Request Percolator"
LABEL org.opencontainers.image.vendor="Alexander Kislitsyn"

WORKDIR /opt/app
COPY --from=build /opt/app/out ./

# Expose service default port
EXPOSE 5000

# Run microservice process.
ENTRYPOINT [ "dotnet", "RequestPercolator.dll" ]