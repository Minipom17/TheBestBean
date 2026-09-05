FROM mcr.microsoft.com/dotnet/sdk:9.0

# Cloud agents need a Debian/Ubuntu-based image. The official SDK image is Debian.
RUN apt-get update \
    && apt-get install -y --no-install-recommends git curl ca-certificates openssh-client tar \
    && rm -rf /var/lib/apt/lists/*
