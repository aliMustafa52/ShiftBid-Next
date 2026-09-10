# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies (leveraging Docker layer cache)
COPY AuctionService.csproj .
RUN dotnet restore AuctionService.csproj

# Copy remaining source code and publish in Release mode
COPY . .
RUN dotnet publish AuctionService.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Configure container port 7001
ENV ASPNETCORE_HTTP_PORTS=7001
EXPOSE 7001

# Copy published binaries from build stage
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AuctionService.dll"]
