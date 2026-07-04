# syntax=docker/dockerfile:1.7

########################################
# Build
########################################
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Arquivos compartilhados da solução
COPY global.json .
COPY Directory.Build.props .
COPY Directory.Packages.props .

# Arquivos de projeto (melhora o aproveitamento do cache do Docker)
COPY src/FcgUsers.Api/FcgUsers.Api.csproj src/FcgUsers.Api/
COPY src/FcgUsers.Application/FcgUsers.Application.csproj src/FcgUsers.Application/
COPY src/FcgUsers.Domain/FcgUsers.Domain.csproj src/FcgUsers.Domain/
COPY src/FcgUsers.Infrastructure/FcgUsers.Infrastructure.csproj src/FcgUsers.Infrastructure/
COPY src/FcgUsers.IoC/FcgUsers.IoC.csproj src/FcgUsers.IoC/
COPY src/FcgUsers.ServiceDefaults/FcgUsers.ServiceDefaults.csproj src/FcgUsers.ServiceDefaults/
COPY src/FcgUsers.SharedKernel/FcgUsers.SharedKernel.csproj src/FcgUsers.SharedKernel/

# Restaura as dependências
RUN dotnet restore src/FcgUsers.Api/FcgUsers.Api.csproj

# Copia o restante do código-fonte
COPY . .

WORKDIR /src/src/FcgUsers.Api

# Publica a aplicação
RUN dotnet publish \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

########################################
# Runtime
########################################
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

RUN apt-get update && apt-get install -y \
    libgssapi-krb5-2 \
    krb5-locales \
    && rm -rf /var/lib/apt/lists/*
    
WORKDIR /app

# Executa como usuário não-root
USER $APP_UID

EXPOSE 7072

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FcgUsers.Api.dll"]