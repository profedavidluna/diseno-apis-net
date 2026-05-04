# Containerización con Docker, CI/CD, Azure y .NET Aspire

Este branch implementa la **contenedorización completa** de LibreriaApp (LibreriaAPI + ApiGateway) con soporte para Docker, pipelines de CI/CD y despliegue en Azure Container Apps, usando .NET Aspire como orquestador de desarrollo.

---

## Estructura de archivos agregados

```
├── LibreriaAPI/
│   └── Dockerfile                    # Multi-stage build para LibreriaAPI
├── ApiGateway/
│   └── Dockerfile                    # Multi-stage build para ApiGateway
├── LibreriaApp.AppHost/              # .NET Aspire AppHost (orquestador)
│   ├── LibreriaApp.AppHost.csproj
│   └── Program.cs
├── LibreriaApp.ServiceDefaults/      # Defaults compartidos de Aspire
│   ├── LibreriaApp.ServiceDefaults.csproj
│   └── Extensions.cs                 # OpenTelemetry, health checks, service discovery
├── infra/
│   └── main.bicep                    # Infraestructura Azure (Container Apps + Log Analytics)
├── .github/workflows/
│   ├── ci.yml                        # CI: build, test y build de imágenes Docker
│   └── cd-azure.yml                  # CD: deploy a Azure Container Apps
├── docker-compose.yml                # Orquestación local (LibreriaAPI + ApiGateway + SQL Server)
└── docker-compose.override.yml       # Overrides para desarrollo local
```

---

## Docker

### Build local

```bash
# Desde la raíz del repositorio
docker build -t libreria-api:local -f LibreriaAPI/Dockerfile .
docker build -t api-gateway:local -f ApiGateway/Dockerfile .
```

### Ejecutar con Docker Compose

```bash
# Levantar todos los servicios: LibreriaAPI + ApiGateway + SQL Server
docker compose up -d

# Ver logs
docker compose logs -f

# Detener
docker compose down
```

| Servicio     | URL local                        |
|-------------|----------------------------------|
| ApiGateway  | `http://localhost:5000`          |
| LibreriaAPI | `http://localhost:8080/swagger`  |

---

## .NET Aspire (desarrollo local)

.NET Aspire orquesta los servicios localmente con un dashboard integrado que muestra logs, trazas y métricas en tiempo real.

### Prerequisitos

```bash
dotnet workload install aspire
```

### Ejecutar con Aspire

```bash
cd LibreriaApp.AppHost
dotnet run
```

El dashboard de Aspire se abre en `https://localhost:15888`. Desde ahí puedes ver:
- Estado de cada servicio (LibreriaAPI, SQL Server)
- Logs estructurados
- Trazas distribuidas (OpenTelemetry)
- Métricas de runtime

---

## CI/CD con GitHub Actions

### Pipeline de CI (`.github/workflows/ci.yml`)

Se ejecuta en cada **push** o **pull request** a `main`:

1. **Build & Test**: compila LibreriaAPI y ApiGateway, ejecuta pruebas
2. **Docker Build**: construye y publica ambas imágenes en GitHub Container Registry (`ghcr.io`)

### Pipeline de CD (`.github/workflows/cd-azure.yml`)

Se ejecuta al hacer **push a `main`** o manualmente:

1. Login en Azure con credenciales (`AZURE_CREDENTIALS` secret)
2. Build y push de ambas imágenes Docker con tag de commit SHA
3. Despliegue de infraestructura con Bicep (`infra/main.bicep`)
4. Verificación de URLs de ambos Container Apps

### Secrets requeridos en GitHub

| Secret | Descripción |
|--------|-------------|
| `AZURE_CREDENTIALS` | JSON con service principal de Azure (`az ad sp create-for-rbac --sdk-auth`) |

---

## Azure Container Apps

La infraestructura se define en `infra/main.bicep` e incluye:

- **Log Analytics Workspace**: telemetría y logs centralizados
- **Container Apps Environment**: ambiente compartido para ambos servicios
- **Container App — LibreriaAPI**: ingress interno, health probes, auto-scaling 1–5 réplicas
- **Container App — ApiGateway**: ingress externo con CORS, punto de entrada público, auto-scaling 1–3 réplicas

### Despliegue manual

```bash
# Crear resource group
az group create --name rg-libreria-app --location eastus

# Desplegar infraestructura
az deployment group create \
  --resource-group rg-libreria-app \
  --template-file infra/main.bicep \
  --parameters \
    libreriaImage=ghcr.io/<owner>/diseno-apis-net/libreria-api:latest \
    gatewayImage=ghcr.io/<owner>/diseno-apis-net/api-gateway:latest \
    containerRegistryServer=ghcr.io \
    containerRegistryUsername=<github-user> \
    containerRegistryPassword=<github-token>
```

---

## Health Checks

LibreriaAPI expone los siguientes endpoints de health (configurados en `ServiceDefaults`):

| Endpoint  | Descripción                      |
|-----------|----------------------------------|
| `/health` | Estado general de la aplicación  |
| `/alive`  | Liveness probe (Azure/K8s)       |
| `/ready`  | Readiness probe (Azure/K8s)      |

---

## Observabilidad (OpenTelemetry)

`LibreriaApp.ServiceDefaults` configura automáticamente:

- **Métricas**: ASP.NET Core, HTTP client, runtime
- **Trazas**: ASP.NET Core, HTTP client
- **Logs**: estructurados con formato y scopes

Para exportar a un colector OTLP externo:

```bash
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
```
