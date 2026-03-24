# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["DbFetcher.csproj", "."]
RUN dotnet restore "DbFetcher.csproj"

# Copy source code
COPY . .

# Build the project
RUN dotnet build "DbFetcher.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "DbFetcher.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Expose port
EXPOSE 5129

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5129
ENV ASPNETCORE_ENVIRONMENT=Production

# Start the application
ENTRYPOINT ["dotnet", "DbFetcher.dll"]
