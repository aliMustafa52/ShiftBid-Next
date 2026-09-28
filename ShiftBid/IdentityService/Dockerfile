# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY IdentityService/IdentityService.csproj IdentityService/
RUN dotnet restore IdentityService/IdentityService.csproj

# Copy remaining source code and publish in Release mode
COPY IdentityService/ IdentityService/
RUN dotnet publish IdentityService/IdentityService.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Configure container port 5000
ENV ASPNETCORE_HTTP_PORTS=5000
EXPOSE 5000

# Copy published binaries from build stage
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "IdentityService.dll"]
