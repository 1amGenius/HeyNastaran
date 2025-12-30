# ========================
# Stage 1: Base image for runtime
# ========================
# Use the lightweight ASP.NET runtime image (suitable for running the app)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set the working directory inside the container
WORKDIR /app

# Expose port 8080 to allow traffic (Telegram webhooks, API calls, etc.)
EXPOSE 8080

# ========================
# Create a non-root user for security
# ========================
# 1. Create a system group named 'appgroup'
# 2. Create a system user 'appuser' in 'appgroup'
# 3. Change ownership of /app to 'appuser:appgroup' so the user has write permissions
RUN addgroup --system appgroup && \
    adduser --system --ingroup appgroup appuser && \
    chown -R appuser:appgroup /app

# Switch to non-root user for security
USER appuser

# ========================
# Stage 2: Build stage
# ========================
# Use the full SDK image for restoring, building, and publishing the .NET project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Allow passing build configuration (default: Release)
ARG BUILD_CONFIGURATION=Release

# Set working directory for build stage
WORKDIR /src

# Copy the project file first and restore dependencies (Docker layer caching)
COPY ["Core.csproj", "."]
RUN dotnet restore "./Core.csproj"

# Copy the rest of the source code into the container
COPY . .

# Set working directory (redundant but ensures correct path)
WORKDIR "/src/."

# Build the project in the specified configuration and output to /app/build
RUN dotnet build "./Core.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ========================
# Stage 3: Publish stage
# ========================
# Publish the project to a folder for deployment
FROM build AS publish

# Use the same build configuration
ARG BUILD_CONFIGURATION=Release

# Publish the app (without self-contained host, smaller image)
RUN dotnet publish "./Core.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ========================
# Stage 4: Final image for production or regular run
# ========================
FROM base AS final

# Set working directory
WORKDIR /app

# Copy published files from the publish stage
COPY --from=publish /app/publish .

# Set the container entrypoint to run the .NET bot
ENTRYPOINT ["dotnet", "Core.dll"]
