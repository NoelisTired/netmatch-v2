# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TARGETARCH
WORKDIR /src

# Copy csproj files first for layer caching
COPY Tourweb.sln ./
COPY Presentation/Presentation.csproj     Presentation/
COPY Logic/Logic.csproj                   Logic/
COPY Interface/Interface.csproj           Interface/
COPY DAL/DAL.csproj                       DAL/
COPY DAL.Fake/DAL.Fake.csproj             DAL.Fake/

RUN dotnet restore Presentation/Presentation.csproj

# Copy source
COPY . .

# Build the Tailwind stylesheet with the standalone CLI (no Node needed).
# Pick the binary that matches the build platform.
ARG TAILWIND_VERSION=v4.1.14
RUN case "${TARGETARCH}" in \
        arm64) TW_ARCH="linux-arm64" ;; \
        *)     TW_ARCH="linux-x64" ;; \
    esac && \
    curl -sL -o /usr/local/bin/tailwindcss \
        "https://github.com/tailwindlabs/tailwindcss/releases/download/${TAILWIND_VERSION}/tailwindcss-${TW_ARCH}" && \
    chmod +x /usr/local/bin/tailwindcss && \
    tailwindcss -i Presentation/Styles/app.css -o Presentation/wwwroot/css/build.css --minify

# Publish
RUN dotnet publish Presentation/Presentation.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# QuestPDF (SkiaSharp) native dependencies
RUN apt-get update && \
    apt-get install -y --no-install-recommends libfontconfig1 && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "Presentation.dll"]
