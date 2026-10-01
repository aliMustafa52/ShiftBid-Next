# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies (leveraging Docker layer cache)
COPY AuctionService/AuctionService.csproj AuctionService/
COPY Contracts/Contracts.csproj Contracts/
RUN dotnet restore AuctionService/AuctionService.csproj

# Copy remaining source code and publish in Release mode
COPY AuctionService/ AuctionService/
COPY Contracts/ Contracts/
RUN dotnet publish AuctionService/AuctionService.csproj -c Release --no-restore -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Configure container port 7001
ENV ASPNETCORE_HTTP_PORTS=7001
EXPOSE 7001

# Copy published binaries from build stage
COPY --from=build /app/publish .

# Allow Wolverine to write generated code while running as the app user
RUN mkdir -p /app/Internal/Generated \
    && chown -R app:app /app/Internal/Generated

USER app

ENTRYPOINT ["dotnet", "AuctionService.dll"]
