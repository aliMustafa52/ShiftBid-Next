# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY SearchService.csproj .
RUN dotnet restore SearchService.csproj

# Copy remaining source code and publish in Release mode
COPY . .
RUN dotnet publish SearchService.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Configure container port 7002
ENV ASPNETCORE_HTTP_PORTS=7002
EXPOSE 7002

# Copy published binaries from build stage
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SearchService.dll"]
