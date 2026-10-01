# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY SearchService/SearchService.csproj SearchService/
COPY Contracts/Contracts.csproj Contracts/
RUN dotnet restore SearchService/SearchService.csproj

# Copy remaining source code and publish in Release mode
COPY SearchService/ SearchService/
COPY Contracts/ Contracts/
RUN dotnet publish SearchService/SearchService.csproj -c Release --no-restore -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Configure container port 7002
ENV ASPNETCORE_HTTP_PORTS=7002
EXPOSE 7002

# Copy published binaries from build stage
COPY --from=build /app/publish .

# Allow Wolverine to write generated code while running as the app user
RUN mkdir -p /app/Internal/Generated \
    && chown -R app:app /app/Internal/Generated

USER app

ENTRYPOINT ["dotnet", "SearchService.dll"]
