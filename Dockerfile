# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER $APP_UID  # If you are using a specific user, uncomment and set APP_UID accordingly
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image with .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the solution and project files for dependency resolution
COPY ["WebApplication1/WebApplication1.csproj", "./"]

# Restore dependencies
RUN dotnet restore "WebApplication1.csproj"

# Copy the rest of the application source code
COPY . ./

# Build the application
RUN dotnet build "WebApplication1.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebApplication1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplication1.dll"]
