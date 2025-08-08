# Devcontainer with latest .NET SDK
FROM mcr.microsoft.com/devcontainers/dotnet

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    npm install -g npm@latest

# Optionale Environmentvariablen
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
    ASPNETCORE_ENVIRONMENT=Development

# Create Workspace-Folder
WORKDIR /workspace

# Install dotnet tools globally
RUN dotnet tool install --global aspire.cli
RUN dotnet tool install --global dotnet-ef
RUN dotnet tool install --global dotnet-outdated-tool 

RUN dotnet new install Aspire.ProjectTemplates --force

RUN dotnet dev-certs https --trust

# Setze PATH für dotnet tools
ENV PATH="$PATH:/root/.dotnet/tools"