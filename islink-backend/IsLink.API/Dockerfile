# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Set environment variables for build optimization
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV NUGET_XMLDOC_MODE=skip

# Copy csproj and restore (with no-cache flag to avoid memory issues)
COPY ["IsLink.API.csproj", "./"]
RUN dotnet restore --no-cache --verbosity quiet

# Copy everything else and build (with memory-efficient flags)
COPY . .
RUN dotnet publish -c Release -o /app/publish \
    --no-restore \
    --nologo \
    /p:UseAppHost=false \
    /p:PublishTrimmed=false \
    /p:EnableCompressionInSingleFile=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "IsLink.API.dll"]

