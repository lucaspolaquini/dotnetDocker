# -------------------------------
# Etapa 1: Imagem base de runtime (leve e segura)
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base

# Define diretório de trabalho para execução da aplicação
WORKDIR /app

# Cria usuário não-root e define como padrão
# UID e GID fixos ajudam em ambientes com volumes compartilhados
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

# Expõe a porta de execução da aplicação (conforme ASPNETCORE_HTTP_PORTS)
EXPOSE 8080

# -------------------------------
# Etapa 2: Build (SDK completo)
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build


ARG BUILD_CONFIGURATION=Release

# Define diretório de trabalho temporário para o build
WORKDIR /src

# Copia apenas o .csproj inicialmente para maximizar o aproveitamento do cache
COPY ["my-api/my-api.csproj", "my-api/"]
RUN dotnet restore "./my-api/my-api.csproj"

# Copia o restante do código
COPY . .

# Define o diretório correto do projeto e compila a aplicação
WORKDIR "/src/my-api"
RUN dotnet build "./my-api.csproj" -c $BUILD_CONFIGURATION -o /app/build


# -------------------------------
# Etapa 3: Publicação
# -------------------------------
FROM build AS publish

ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./my-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -------------------------------
# Etapa 4: Imagem final
# -------------------------------
FROM base AS final
WORKDIR /app

# Copia os artefatos publicados da etapa anterior
COPY --from=publish /app/publish .

# Define o ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "my-api.dll"]