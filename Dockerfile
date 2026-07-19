# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file first
COPY CorporateWebsite.sln .

# Copy project files
COPY CorporateWebsite.Core/CorporateWebsite.Core.csproj CorporateWebsite.Core/
COPY CorporateWebsite.Application/CorporateWebsite.Application.csproj CorporateWebsite.Application/
COPY CorporateWebsite.Infrastructure/CorporateWebsite.Infrastructure.csproj CorporateWebsite.Infrastructure/
COPY CorporateWebsite.Web/CorporateWebsite.Web.csproj CorporateWebsite.Web/

# Restore dependencies
RUN dotnet restore CorporateWebsite.sln

# Copy all source code
COPY . .

# Build and publish
RUN dotnet publish CorporateWebsite.Web/CorporateWebsite.Web.csproj -c Release -o /app/publish --no-restore /p:PublishTrimmed=false /p:PublishSingleFile=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install runtime dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    ca-certificates \
    libgdiplus \
    libc6-dev \
    tzdata \
    fontconfig \
    fonts-dejavu-core \
    && rm -rf /var/lib/apt/lists/* \
    && fc-cache -f -v

# Set timezone
ENV TZ=Asia/Tehran
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Create non-root user
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app
USER appuser

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "CorporateWebsite.Web.dll"]