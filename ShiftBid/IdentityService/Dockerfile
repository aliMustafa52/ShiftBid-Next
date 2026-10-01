# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY IdentityService/IdentityService.csproj IdentityService/
RUN dotnet restore IdentityService/IdentityService.csproj

# Copy remaining source code and publish in Release mode
COPY IdentityService/ IdentityService/
RUN dotnet publish IdentityService/IdentityService.csproj -c Release --no-restore -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Configure container port 5000
ENV ASPNETCORE_HTTP_PORTS=5000 \
    DataProtection__KeysPath=/app/data-protection-keys
EXPOSE 5000

# Copy published binaries from build stage
COPY --from=build /app/publish .

# Give the app user access to its persistent key stores
RUN mkdir -p /app/keys /app/data-protection-keys \
    && chown -R app:app /app/keys /app/data-protection-keys \
    && chmod 700 /app/keys /app/data-protection-keys

USER app

ENTRYPOINT ["dotnet", "IdentityService.dll"]
