FROM mcr.microsoft.com/dotnet/sdk:3.1 as build-env
WORKDIR /app

# Copying the application
COPY . ./
# dev-certs
RUN dotnet dev-certs https
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Debug -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1

# Copying & Setting opa permissions
RUN curl -L -o /usr/bin/opa https://openpolicyagent.org/downloads/v0.41.0/opa_linux_amd64_static
RUN chmod 755 /usr/bin/opa

# Building runtime image
WORKDIR /app
COPY --from=build-env /app/out .

# Opening docker port to the application
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT [ "dotnet", "web-app-example.dll" ]