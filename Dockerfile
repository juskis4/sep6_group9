# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source

# Copy everything and build the project
COPY . ./
RUN dotnet restore "./webApplication/webApplication.csproj" --disable-parallel
# All of the artifacts will end up in the /app
RUN dotnet publish "./webApplication/webApplication.csproj" -c Release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app
COPY --from=build /app ./

# The application will use the port defined in the PORT environment variable
EXPOSE $PORT

ENTRYPOINT ["dotnet", "webApplication.dll"]
