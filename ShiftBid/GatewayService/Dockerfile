# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies (leveraging Docker layer cache)
COPY GatewayService/GatewayService.csproj GatewayService/
RUN dotnet restore GatewayService/GatewayService.csproj

# Copy remaining source code and publish in Release mode
COPY GatewayService/ GatewayService/
RUN dotnet publish GatewayService/GatewayService.csproj -c Release --no-restore -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Configure container port 6001
ENV ASPNETCORE_HTTP_PORTS=6001
EXPOSE 6001

# Copy published binaries from build stage
COPY --from=build /app/publish .

USER app

ENTRYPOINT ["dotnet", "GatewayService.dll"]
