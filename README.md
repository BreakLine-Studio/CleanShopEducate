# CleanShopEducate

**CleanShopEducate** es un proyecto educativo que demuestra cómo construir una API de e-commerce sencilla aplicando **Clean Architecture**, **DDD táctico ligero**, **MediatR** (CQRS), **EF Core**, pruebas automatizadas y buenas prácticas para equipos en formación.

> Objetivo: servir como plantilla didáctica para enseñar diseño, arquitectura, pruebas y flujos de trabajo modernos en .NET.

------

## 🧱 Tech Stack

- **.NET** 8 (o superior) — ASP.NET Core Web API
- **EF Core** — Migrations, `DbContext`, Repositorios
- **MediatR** — Commands/Queries, Handlers y Pipeline Behaviors
- **FluentValidation** — Validaciones de entrada
- **AutoMapper** — Mapeo DTOs ↔ Entidades
- **Swagger / OpenAPI** — Documentación de la API
- **xUnit / NUnit** + **Moq** (o equivalente) — Testing
- **Docker / Docker Compose** — entorno reproducible
- **PostgreSQL** (por defecto) — base de datos relacional

> Si tu implementación usa SQL Server o MySQL, solo cambia la cadena de conexión y el proveedor de EF.

------

## ✨ Funcionalidades (MVP)

- Gestión de **Productos**: crear, consultar por id, listar, actualizar, eliminar.
- Validación de **SKU** único (dominio).
- Gestión de **Stock** y **Precio** con invariantes simples.
- Separación de **dominio**, **aplicación**, **infraestructura** y **API**.

------

## 🏗️ Arquitectura

```
flowchart LR
    UI[API (Controllers/Minimal API)] -->|DTOs / Mapeos| APP[Application]
    APP -->|Commands/Queries| MEDIATR[(MediatR)]
    MEDIATR -->|Handlers| DOM[Domain]
    APP -->|Repos Abstraídos| INFRA[Infrastructure]
    INFRA -->|EF Core| DB[(PostgreSQL)]
```

**Capas**

- **Domain**: Entidades, Value Objects (p.ej. `Sku`), Reglas de negocio, Excepciones de dominio.
- **Application**: Casos de uso (Handlers MediatR), DTOs, Validaciones, Puertos (interfaces de repositorios, UoW).
- **Infrastructure**: EF Core (`DbContext`), Repositorios concretos, Unit of Work, Migrations, proveedores externos.
- **API**: Endpoints/Controllers, mapeos, filtros globales de errores, Swagger.

------

## 📂 Estructura sugerida del repo

```
CleanShopEducate/
 ├─ src/
 │   ├─ CleanShop.Api/
 │   ├─ CleanShop.Application/
 │   ├─ CleanShop.Domain/
 │   └─ CleanShop.Infrastructure/
 ├─ tests/
 │   ├─ CleanShop.UnitTests/
 │   └─ CleanShop.IntegrationTests/
 ├─ docker/
 │   ├─ docker-compose.yml
 │   └─ postgres/
 ├─ .editorconfig
 ├─ .gitignore
 └─ README.md
```

> Si tu repo ya tiene otra distribución (por ejemplo, carpeta `app`, `core`, `infra`), la adaptamos en dos minutos.

------

## ⚙️ Requisitos

- **.NET SDK** 8+
- **Docker** y **Docker Compose** (opcional pero recomendado)
- **PostgreSQL** 14+ (si no usas Docker)

------

## 🚀 Puesta en marcha (modo rápido con Docker)

1. **Levanta la base de datos**

```
docker compose -f docker/docker-compose.yml up -d
```

1. **Aplicar migraciones / crear DB**

```
dotnet tool restore
dotnet build
dotnet ef database update --project src/CleanShop.Infrastructure --startup-project src/CleanShop.Api
```

1. **Ejecutar la API**

```
dotnet run --project src/CleanShop.Api
```

1. **Explorar Swagger**
    Abre: `http://localhost:8080/swagger` (o el puerto que definas).

------

## 🔧 Configuración

**appsettings.Development.json** (ejemplo PostgreSQL)

```
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=cleanshop;Username=postgres;Password=postgres"
  },
  "Swagger": { "Enabled": true },
  "Serilog": { "MinimumLevel": "Information" }
}
```

> Cambia a `SqlServer`/`MySql` si corresponde e instala el provider EF Core adecuado.

------

## 🧩 Casos de uso (ejemplo Productos)

### Comando: Crear producto

- **Request**: `POST /api/products`

```
{
  "name": "Mouse inalámbrico",
  "sku": "MS-001",
  "price": 59.99,
  "stock": 25
}
```

- **Reglas**:
  - `Sku` debe ser único (se valida en dominio + repo).
  - `Price` ≥ 0, `Stock` ≥ 0.
- **Handler** (simplificado): valida SKU, crea `Product`, persiste y retorna `Id`.

### Query: Obtener por Id

- **Request**: `GET /api/products/{id}`
- **Response**: DTO del producto.

> Agrega endpoints de listado con paginación y filtros según tus necesidades.

------

## ✅ Pruebas

### Unit Tests

- **Domain**: invariantes (`Sku`, `Product`), reglas de negocio.
- **Application**: `CreateProductHandler` con repos mockeados (MediatR + Moq).
- **Validations**: FluentValidation para DTOs/Commands.

### Integration Tests

- **Infra + DB**: repositorios concretos con DB local o Testcontainers.
- **API**: `WebApplicationFactory` para probar endpoints end-to-end.

Comandos típicos:

```
dotnet test
```

------

## 📜 Estándares y calidad

- **SonarLint / Analyzers**: mantener deuda técnica baja.
- **Convenciones**: nombres consistentes, DTOs sin lógica, entidades ricas en dominio.
- **Errores globales**: `ProblemDetails` + mapeo de excepciones (dominio/validación/autenticación/externos).

------

## 📚 Scripts útiles (EF Core)

```
# Crear nueva migración
dotnet ef migrations add Init --project src/CleanShop.Infrastructure --startup-project src/CleanShop.Api

# Revertir última
dotnet ef database update LastGoodMigrationName

# Eliminar DB (desarrollo)
dotnet ef database drop --force --project src/CleanShop.Infrastructure --startup-project src/CleanShop.Api
```

------

## 🧪 Datos semilla (opcional)

Puedes agregar una clase `DataSeeder` en `Infrastructure` para insertar productos demo al iniciar en `Development`.

------

## 🐳 Docker (API)

`docker/docker-compose.yml` (fragmento ilustrativo)

```yaml
services:
  db:
    image: postgres:16
    environment:
      POSTGRES_DB: cleanshop
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5433:5432"
    volumes:
      - cleanshop_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres -d cleanshop"]
      interval: 5s
      timeout: 3s
      retries: 10

  api:
    build:
      context: ..                        # RAÍZ del repo (donde están todas las carpetas CleanShop.*)
      dockerfile: CleanShop.Api/Api.Dockerfile
    environment:
      # .NET 8+ soporta ASPNETCORE_HTTP_PORTS como alternativa simple.
      ASPNETCORE_HTTP_PORTS: "8080"   # o usa ASPNETCORE_URLS si prefieres
      ASPNETCORE_ENVIRONMENT: "Development"
      ConnectionStrings__Default: "Host=db;Port=5433;Database=cleanshop;Username=postgres;Password=postgres"
    ports:
      - "8081:8080"
    depends_on:
      db:
        condition: service_healthy

volumes:
  cleanshop_data:
```

------

Dockerfile

> El archivo dockerfile se debe crear en la solucion webAPI con el nombre de la solucion seguido de punto y DockerFile. Ejemplo practico del repositorio Api.Dockerfile

```dockerfile
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

```

DockerIgnore

> Se crea en la solucion webApi

```
bin
obj
**/*.user
**/*.suo
**/*.cache
**/*.log
```

Como ejecutar

## ⚙️ Usando Docker / Docker Compose

1. Abre terminal en la carpeta raíz de tu proyecto (la que contiene `docker/` y la solución `.sln`).

2. Construye las imágenes Docker:

   ```
   docker compose -f docker/docker-compose.yml build
   ```

3. Levanta los contenedores (API + base de datos):

   ```
   docker compose -f docker/docker-compose.yml up -d
   ```

4. Verifica que esté corriendo:

   ```
   docker compose -f docker/docker-compose.yml ps
   ```

   Debes ver algo como `api` y `db` con estado “Up”.

5. En tu navegador o Postman entra a la URL que mapeaste, por ejemplo:

   ```
   http://localhost:8081/swagger
   ```

   (o el puerto que hayas decidido en `ports:` para el servicio `api`).

6. Cuando quieras detener:

   ```
   docker compose -f docker/docker-compose.yml down
   ```

## 🔨 Ejecutando localmente sin Docker (solo .NET)

Si quieres correr solo la API desde tu máquina, sin contenedores:

1. Asegúrate de tener el SDK .NET 9 instalado.

2. En la terminal, entra a la carpeta `CleanShop.Api`:

   ```
   cd CleanShop.Api
   ```

3. Restaura dependencias:

   ```
   dotnet restore
   ```

4. (Opcional) Corre migraciones si usas EF Core:

   ```
   dotnet ef database update
   ```

5. Ejecuta:

   ```
   dotnet run
   ```

6. Luego, abre tu navegador/postman en:

   ```
   http://localhost:5000/swagger
   ```

   (o el puerto que el proyecto indique, puede estar configurado en `launchSettings.json` o en `appsettings`).

## 🧭 Roadmap educativo

-  Módulo de **categorías** con relación 1-N a productos.
-  **Autenticación** (JWT o cookies seguras) y autorización por roles.
-  **Pipelines MediatR**: Logging, Validation y Performance.
-  **Observabilidad**: Serilog + OpenTelemetry.
-  **Front-end demo** (Angular/React) consumiendo la API.

------

## 🤝 Contribuir

1. Haz un fork y crea una rama `feature/mi-mejora`.
2. Añade/ajusta pruebas.
3. Abre un PR describiendo cambio, motivación y captura de Swagger si aplica.

------

## 📄 Licencia

Este proyecto educativo se distribuye bajo la licencia **MIT** (o la que prefieras).
 Incluye el archivo `LICENSE` en la raíz.

------

## 🆘 Soporte

- Problemas o preguntas: abre un **Issue**.
- ¿Quieres que el README refleje exactamente tu estructura y scripts del repo? Pásame:
  - El **árbol** de carpetas (`tree -L 3` o captura),
  - Tu `appsettings*.json`,
  - Los comandos que usas para `migrations` y `run`.