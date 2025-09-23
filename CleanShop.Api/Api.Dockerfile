# ---------- Runtime base ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
# En .NET 8+ la imagen expone 8080 por defecto. Puedes mapearlo desde compose. :contentReference[oaicite:0]{index=0}

# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos primero los .csproj para aprovechar caché
COPY CleanShop.Api/CleanShop.Api.csproj CleanShop.Api/
COPY CleanShop.Application/CleanShop.Application.csproj CleanShop.Application/
COPY CleanShop.Domain/CleanShop.Domain.csproj CleanShop.Domain/
COPY CleanShop.Infrastructure/CleanShop.Infrastructure.csproj CleanShop.Infrastructure/

RUN dotnet restore CleanShop.Api/CleanShop.Api.csproj

# Copiamos el resto del código
COPY . .

# Compilamos
WORKDIR /src/CleanShop.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---------- Final ----------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CleanShop.Api.dll"]
