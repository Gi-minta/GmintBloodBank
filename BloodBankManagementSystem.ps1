<#
===========================================================
 Blood Bank Management System — Starter Kit
 Setup Script

 Plataforma:
   - .NET 10
   - React 19 + TypeScript
   - PostgreSQL 17
   - Docker
   - Clean Architecture + DDD + CQRS

 Uso:
   ./BloodBankManagementSystem.ps1
   ./BloodBankManagementSystem.ps1 -ProjectName "MiBanco" -DbPassword "S3cur3!"
   ./BloodBankManagementSystem.ps1 -UseDocker $false
===========================================================
#>


param(
    [string]$ProjectName = "BloodBankManagementSystem",
    [bool]$UseDocker     = $true,
    [bool]$UseFrontend   = $true,

    # Credenciales parametrizadas — CAMBIAR antes de producción
    [string]$DbPassword      = "CHANGE_ME_IN_PRODUCTION",
    [string]$PgAdminEmail    = "admin@bloodbank.com",
    [string]$PgAdminPassword = "CHANGE_ME_IN_PRODUCTION",
    [string]$JwtSecret       = "CHANGE_ME_MIN_32_CHARS_IN_PRODUCTION"
)


# =========================================================
# Variables globales de rutas
# =========================================================

$ErrorActionPreference = "Stop"

$Root         = Join-Path (Get-Location) $ProjectName
$Backend      = "$Root\Backend"
$Frontend     = "$Root\Frontend"
$Database     = "$Root\Database"
$DockerFolder = "$Root\Docker"
$Docs         = "$Root\Docs"
$Tests        = "$Root\Tests"


# =========================================================
# Funciones de consola
# =========================================================

function Write-Info {
    param($Message)
    Write-Host "[INFO] $Message" -ForegroundColor Cyan
}

function Write-Success {
    param($Message)
    Write-Host "[OK]   $Message" -ForegroundColor Green
}

function Write-ErrorMessage {
    param($Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

function Write-Section {
    param($Title)
    Write-Host ""
    Write-Host "=============================================" -ForegroundColor DarkGray
    Write-Host " $Title" -ForegroundColor White
    Write-Host "=============================================" -ForegroundColor DarkGray
}


# =========================================================
# Validar herramientas instaladas
# =========================================================

function Test-Command {
    param(
        [string]$Command,
        [bool]$Required = $true
    )

    $exists = Get-Command $Command -ErrorAction SilentlyContinue

    if (-not $exists)
    {
        if ($Required)
        {
            throw "$Command no está instalado. Instálalo antes de continuar."
        }
        else
        {
            Write-Host "[WARN] $Command no encontrado — funcionalidad opcional omitida." `
                -ForegroundColor Yellow
            return $false
        }
    }

    return $true
}


function Test-Prerequisites {

    Write-Section "Validando herramientas"

    Test-Command "dotnet" -Required $true
    Test-Command "git"    -Required $true

    if ($UseFrontend)
    {
        $npmFound = Test-Command "npm" -Required $true
        if (-not $npmFound)
        {
            throw "npm es requerido para generar el Frontend. Instala Node.js o ejecuta con -UseFrontend `$false"
        }
    }

    if ($UseDocker)
    {
        Test-Command "docker" -Required $false | Out-Null
    }

    Write-Success "Entorno validado correctamente."
}


# =========================================================
# Crear carpetas raíz
# =========================================================

function Initialize-Folders {

    Write-Section "Estructura de carpetas"

    @(
        $Root,
        $Backend,
        $Frontend,
        $Database,
        $DockerFolder,
        $Docs,
        $Tests
    ) | ForEach-Object {
        if (!(Test-Path $_))
        {
            New-Item -ItemType Directory -Path $_ -Force | Out-Null
        }
    }

    @(
        "$Database\Scripts",
        "$Database\Backups",

        "$Docs\API",
        "$Docs\Database",
        "$Docs\Architecture",
        "$Docs\Diagrams",

        "$DockerFolder\PostgreSQL",
        "$DockerFolder\PgAdmin"
    ) | ForEach-Object {
        New-Item -ItemType Directory -Path $_ -Force | Out-Null
    }

    Write-Success "Estructura principal creada."
}


# =========================================================
# Crear solución .NET
# =========================================================

function Create-Solution {

    Write-Section "Solución .NET"

    Push-Location $Backend

    dotnet new sln -n BloodBank

    Write-Success "Solución BloodBank.sln creada."

    Pop-Location
}


# =========================================================
# Crear proyectos Clean Architecture
# =========================================================

function Create-BackendProjects {

    Write-Section "Proyectos Backend"

    Push-Location $Backend

    dotnet new webapi   -n API
    dotnet new classlib -n Domain
    dotnet new classlib -n Application
    dotnet new classlib -n Infrastructure
    dotnet new xunit    -n Tests

    Write-Success "Proyectos creados."

    Write-Info "Agregando proyectos a la solución..."

    dotnet sln BloodBank.sln add API/API.csproj
    dotnet sln BloodBank.sln add Domain/Domain.csproj
    dotnet sln BloodBank.sln add Application/Application.csproj
    dotnet sln BloodBank.sln add Infrastructure/Infrastructure.csproj
    dotnet sln BloodBank.sln add Tests/Tests.csproj

    Write-Success "Proyectos registrados en la solución."

    Pop-Location
}


# =========================================================
# Estructura interna Clean Architecture
# =========================================================

function Create-CleanArchitectureFolders {

    Write-Section "Estructura Clean Architecture"

    $folders = @(

        # API
        "$Backend\API\Controllers",
        "$Backend\API\Middleware",
        "$Backend\API\Extensions",
        "$Backend\API\Filters",
        "$Backend\API\HealthChecks",
        "$Backend\API\Swagger",

        # Domain — por agregado
        "$Backend\Domain\Aggregates\UserAggregate",
        "$Backend\Domain\Aggregates\DonorAggregate",
        "$Backend\Domain\Aggregates\DonationAggregate",
        "$Backend\Domain\Aggregates\BloodUnitAggregate",
        "$Backend\Domain\Enums",
        "$Backend\Domain\Events",
        "$Backend\Domain\Exceptions",
        "$Backend\Domain\ValueObjects",
        "$Backend\Domain\Common",

        # Application
        "$Backend\Application\Common",
        "$Backend\Application\Common\Interfaces",
        "$Backend\Application\Common\Behaviors",
        "$Backend\Application\Common\Exceptions",
        "$Backend\Application\Common\Models",

        "$Backend\Application\DTOs",

        # Features por módulo de negocio
        "$Backend\Application\Features\Auth\Commands",
        "$Backend\Application\Features\Auth\Queries",

        "$Backend\Application\Features\Donors\Commands",
        "$Backend\Application\Features\Donors\Queries",

        "$Backend\Application\Features\Donations\Commands",
        "$Backend\Application\Features\Donations\Queries",

        "$Backend\Application\Features\BloodUnits\Commands",
        "$Backend\Application\Features\BloodUnits\Queries",

        "$Backend\Application\Features\Inventory\Commands",
        "$Backend\Application\Features\Inventory\Queries",

        "$Backend\Application\Features\Reports\Queries",

        # Infrastructure
        "$Backend\Infrastructure\Persistence",
        "$Backend\Infrastructure\Persistence\Migrations",
        "$Backend\Infrastructure\Persistence\Configurations",
        "$Backend\Infrastructure\Persistence\Seeds",
        "$Backend\Infrastructure\Repositories",
        "$Backend\Infrastructure\Services",
        "$Backend\Infrastructure\Security",
        "$Backend\Infrastructure\Logging"
    )

    foreach ($folder in $folders)
    {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
    }

    Write-Success "Estructura Clean Architecture + DDD creada."
}


# =========================================================
# Configurar referencias entre proyectos
# =========================================================

function Configure-ProjectReferences {

    Write-Section "Referencias entre proyectos"

    Push-Location $Backend

    # API → Application + Infrastructure (punto de entrada)
    dotnet add API/API.csproj reference Application/Application.csproj
    dotnet add API/API.csproj reference Infrastructure/Infrastructure.csproj

    # Application → Domain (solo depende del dominio)
    dotnet add Application/Application.csproj reference Domain/Domain.csproj

    # Infrastructure → Application + Domain
    dotnet add Infrastructure/Infrastructure.csproj reference Application/Application.csproj
    dotnet add Infrastructure/Infrastructure.csproj reference Domain/Domain.csproj

    # Tests → API (pruebas de integración)
    dotnet add Tests/Tests.csproj reference API/API.csproj

    Write-Success "Referencias configuradas. Capas: Domain ← Application ← Infrastructure ← API"

    Pop-Location
}


# =========================================================
# Instalar paquetes NuGet por capa
# =========================================================

function Install-NuGetPackages {

    Write-Section "Paquetes NuGet"

    Push-Location $Backend

    # --- Domain: sin dependencias externas ---

    # --- Application: contratos + comportamientos ---
    $appPackages = @(
        "MediatR",
        "FluentValidation",
        "FluentValidation.DependencyInjectionExtensions"
    )

    foreach ($pkg in $appPackages)
    {
        dotnet add Application/Application.csproj package $pkg
    }

    Write-Success "Paquetes Application instalados."

    # --- Infrastructure: EF Core + Postgres + seguridad ---
    $infraPackages = @(
        "Microsoft.EntityFrameworkCore",
        "Microsoft.EntityFrameworkCore.Design",
        "Npgsql.EntityFrameworkCore.PostgreSQL",
        "Serilog.AspNetCore"
    )

    foreach ($pkg in $infraPackages)
    {
        dotnet add Infrastructure/Infrastructure.csproj package $pkg
    }

    Write-Success "Paquetes Infrastructure instalados."

    # --- API: capa de presentación ---
    $apiPackages = @(
        "Microsoft.AspNetCore.Authentication.JwtBearer",
        "Swashbuckle.AspNetCore",
        "AutoMapper",
        "AutoMapper.Extensions.Microsoft.DependencyInjection"
    )

    foreach ($pkg in $apiPackages)
    {
        dotnet add API/API.csproj package $pkg
    }

    Write-Success "Paquetes API instalados."

    # --- Tests ---
    dotnet add Tests/Tests.csproj package Microsoft.AspNetCore.Mvc.Testing
    dotnet add Tests/Tests.csproj package FluentAssertions

    Write-Success "Paquetes Tests instalados."

    Pop-Location
}


# =========================================================
# Helper: crear archivo .cs vacío
# =========================================================

function New-CsFile {
    param($Path)
    if (!(Test-Path $Path))
    {
        New-Item -ItemType File -Path $Path -Force | Out-Null
    }
}


# =========================================================
# Crear entidades del dominio (por agregado)
# =========================================================

function Create-DomainFiles {

    Write-Section "Capa Domain"

    # UserAggregate
    @("User", "Role", "RefreshToken") | ForEach-Object {
        New-CsFile "$Backend\Domain\Aggregates\UserAggregate\$_.cs"
    }

    # DonorAggregate
    @("Donor", "DonorMedicalCondition", "MedicalCondition") | ForEach-Object {
        New-CsFile "$Backend\Domain\Aggregates\DonorAggregate\$_.cs"
    }

    # DonationAggregate
    @("DonationAppointment", "DonationEvaluation", "Donation") | ForEach-Object {
        New-CsFile "$Backend\Domain\Aggregates\DonationAggregate\$_.cs"
    }

    # BloodUnitAggregate
    @("BloodUnit", "BloodScreening", "InventoryMovement") | ForEach-Object {
        New-CsFile "$Backend\Domain\Aggregates\BloodUnitAggregate\$_.cs"
    }

    # Enums
    @("BloodType", "DonationStatus", "BloodUnitStatus", "InventoryMovementType") | ForEach-Object {
        New-CsFile "$Backend\Domain\Enums\$_.cs"
    }

    # Value Objects
    @("BloodPressure", "PersonName", "Address") | ForEach-Object {
        New-CsFile "$Backend\Domain\ValueObjects\$_.cs"
    }

    # Base
    @("Entity", "AggregateRoot", "DomainEvent") | ForEach-Object {
        New-CsFile "$Backend\Domain\Common\$_.cs"
    }

    Write-Success "Archivos Domain generados por agregado."
}


# =========================================================
# Crear capa Application
# =========================================================

function Create-ApplicationFiles {

    Write-Section "Capa Application"

    # Interfaces comunes
    @("IRepository", "IUnitOfWork", "ICurrentUserService", "IJwtService", "IAuditService") | ForEach-Object {
        New-CsFile "$Backend\Application\Common\Interfaces\$_.cs"
    }

    # Modelos comunes
    @("Result", "PagedResult", "PaginationQuery") | ForEach-Object {
        New-CsFile "$Backend\Application\Common\Models\$_.cs"
    }

    # Behaviors
    @("ValidationBehavior", "LoggingBehavior", "TransactionBehavior") | ForEach-Object {
        New-CsFile "$Backend\Application\Common\Behaviors\$_.cs"
    }

    # DTOs
    @("DonorDto", "DonationDto", "BloodUnitDto", "UserDto") | ForEach-Object {
        New-CsFile "$Backend\Application\DTOs\$_.cs"
    }

    # Auth
    New-CsFile "$Backend\Application\Features\Auth\Commands\LoginCommand.cs"
    New-CsFile "$Backend\Application\Features\Auth\Commands\RefreshTokenCommand.cs"

    # Donors — en su carpeta correcta
    New-CsFile "$Backend\Application\Features\Donors\Commands\CreateDonorCommand.cs"
    New-CsFile "$Backend\Application\Features\Donors\Commands\UpdateDonorCommand.cs"
    New-CsFile "$Backend\Application\Features\Donors\Queries\GetDonorsQuery.cs"
    New-CsFile "$Backend\Application\Features\Donors\Queries\GetDonorByIdQuery.cs"

    # Donations — en su carpeta correcta
    New-CsFile "$Backend\Application\Features\Donations\Commands\CreateDonationCommand.cs"
    New-CsFile "$Backend\Application\Features\Donations\Commands\RegisterEvaluationCommand.cs"
    New-CsFile "$Backend\Application\Features\Donations\Queries\GetDonationsQuery.cs"

    # BloodUnits
    New-CsFile "$Backend\Application\Features\BloodUnits\Commands\ReleaseBloodUnitCommand.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Commands\DiscardBloodUnitCommand.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Queries\GetAvailableInventoryQuery.cs"

    # Inventory
    New-CsFile "$Backend\Application\Features\Inventory\Queries\GetInventoryQuery.cs"

    # Reports
    New-CsFile "$Backend\Application\Features\Reports\Queries\GetBloodStockSummaryQuery.cs"

    Write-Success "Capa Application creada con features por módulo."
}


# =========================================================
# Archivos de configuración raíz
# =========================================================

function Create-ConfigurationFiles {

    Write-Section "Configuración base"

    Create-GitIgnore
    Create-EditorConfig
    Create-Readme

    Write-Success "Archivos de configuración creados."
}


# =========================================================
# Frontend React 19 + Vite + TypeScript
# =========================================================

function Create-Frontend {

    Write-Section "Frontend React 19"

    Push-Location $Root

    npm create vite@latest Frontend -- --template react-ts

    Write-Success "Proyecto React 19 + TypeScript creado."

    Push-Location "$Root\Frontend"

    Write-Info "Instalando dependencias React..."

    $packages = @(
        "react-router-dom",
        "axios",
        "@tanstack/react-query",
        "zustand",
        "react-hook-form",
        "zod",
        "@hookform/resolvers",
        "dayjs",
        "clsx"
    )

    foreach ($pkg in $packages)
    {
        npm install $pkg
    }

    npm install tailwindcss @tailwindcss/vite

    Write-Success "Dependencias instaladas."

    Pop-Location
    Pop-Location
}


function Create-FrontendFolders {

    Write-Info "Creando estructura Frontend..."

    $folders = @(
        "$Root\Frontend\src\api",
        "$Root\Frontend\src\assets",

        "$Root\Frontend\src\components\common",
        "$Root\Frontend\src\components\forms",
        "$Root\Frontend\src\components\layout",

        "$Root\Frontend\src\pages\Auth",
        "$Root\Frontend\src\pages\Dashboard",
        "$Root\Frontend\src\pages\Donors",
        "$Root\Frontend\src\pages\Donations",
        "$Root\Frontend\src\pages\BloodUnits",
        "$Root\Frontend\src\pages\Inventory",
        "$Root\Frontend\src\pages\Reports",

        "$Root\Frontend\src\routes",
        "$Root\Frontend\src\hooks",
        "$Root\Frontend\src\store",
        "$Root\Frontend\src\types",
        "$Root\Frontend\src\utils",
        "$Root\Frontend\src\config",
        "$Root\Frontend\src\constants"
    )

    foreach ($folder in $folders)
    {
        if (!(Test-Path $folder))
        {
            New-Item -ItemType Directory -Path $folder -Force | Out-Null
        }
    }

    Write-Success "Estructura React creada."
}


function New-TsFile {
    param($Path)
    if (!(Test-Path $Path))
    {
        New-Item -ItemType File -Path $Path -Force | Out-Null
    }
}


function Create-FrontendFiles {

    Write-Info "Creando archivos base React..."

    $files = @(
        "$Root\Frontend\src\api\axios.ts",
        "$Root\Frontend\src\config\env.ts",

        "$Root\Frontend\src\routes\AppRoutes.tsx",
        "$Root\Frontend\src\routes\PrivateRoute.tsx",

        "$Root\Frontend\src\store\authStore.ts",

        "$Root\Frontend\src\hooks\useAuth.ts",

        "$Root\Frontend\src\types\User.ts",
        "$Root\Frontend\src\types\Donor.ts",
        "$Root\Frontend\src\types\Donation.ts",
        "$Root\Frontend\src\types\BloodUnit.ts",

        "$Root\Frontend\src\utils\date.ts",
        "$Root\Frontend\src\utils\validators.ts",

        "$Root\Frontend\src\pages\Auth\Login.tsx",
        "$Root\Frontend\src\pages\Dashboard\Dashboard.tsx",
        "$Root\Frontend\src\pages\Donors\DonorList.tsx",
        "$Root\Frontend\src\pages\Donors\DonorForm.tsx",
        "$Root\Frontend\src\pages\Donations\DonationList.tsx",
        "$Root\Frontend\src\pages\Donations\DonationForm.tsx",
        "$Root\Frontend\src\pages\BloodUnits\BloodUnitList.tsx",
        "$Root\Frontend\src\pages\Inventory\InventoryList.tsx",
        "$Root\Frontend\src\pages\Reports\Reports.tsx"
    )

    foreach ($file in $files)
    {
        New-TsFile $file
    }

    Write-Success "Archivos React generados."
}


function Create-FrontendEnvironment {

    Write-Info "Creando .env Frontend..."

    $envFile = "$Root\Frontend\.env"

@"
VITE_API_URL=http://localhost:5000/api
VITE_APP_NAME=Blood Bank Management System
VITE_VERSION=1.0.0
"@ | Set-Content $envFile

    Write-Success ".env Frontend creado."
}


# =========================================================
# Docker
# =========================================================

function Create-ApiDockerfile {

    Write-Info "Creando Dockerfile API..."

    $dockerfile = "$Backend\API\Dockerfile"

@"
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore API/API.csproj

RUN dotnet publish API/API.csproj \
    -c Release \
    -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "API.dll"]
"@ | Set-Content $dockerfile

    Write-Success "Dockerfile API creado."
}


function Create-FrontendDockerfile {

    Write-Info "Creando Dockerfile Frontend..."

    $dockerfile = "$Frontend\Dockerfile"

@"
FROM node:22-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

RUN npm run build


FROM nginx:alpine

COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
"@ | Set-Content $dockerfile

    Write-Success "Dockerfile Frontend creado."
}


function Create-PostgreSQLConfiguration {

    Write-Info "Configurando PostgreSQL..."

    $postgresEnv = "$DockerFolder\PostgreSQL\.env"

@"
POSTGRES_DB=BloodBankDB
POSTGRES_USER=postgres
POSTGRES_PASSWORD=$DbPassword
"@ | Set-Content $postgresEnv

    Write-Success "Configuración PostgreSQL creada."
}


function Create-PgAdminConfiguration {

    Write-Info "Configurando PgAdmin..."

    $pgAdminEnv = "$DockerFolder\PgAdmin\.env"

@"
PGADMIN_DEFAULT_EMAIL=$PgAdminEmail
PGADMIN_DEFAULT_PASSWORD=$PgAdminPassword
"@ | Set-Content $pgAdminEnv

    Write-Success "Configuración PgAdmin creada."
}


function Create-DockerCompose {

    Write-Info "Creando docker-compose.yml..."

    $compose = "$DockerFolder\docker-compose.yml"

@"
version: '3.9'

services:

  postgres:
    image: postgres:17
    container_name: bloodbank-postgres
    restart: unless-stopped
    env_file:
      - ./PostgreSQL/.env
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - bloodbank-network

  pgadmin:
    image: dpage/pgadmin4
    container_name: bloodbank-pgadmin
    restart: unless-stopped
    env_file:
      - ./PgAdmin/.env
    ports:
      - "5050:80"
    depends_on:
      - postgres
    networks:
      - bloodbank-network

  api:
    build:
      context: ../Backend
      dockerfile: API/Dockerfile
    container_name: bloodbank-api
    ports:
      - "5000:8080"
    depends_on:
      - postgres
    networks:
      - bloodbank-network

  frontend:
    build:
      context: ../Frontend
    container_name: bloodbank-web
    ports:
      - "3000:80"
    depends_on:
      - api
    networks:
      - bloodbank-network

volumes:
  postgres_data:

networks:
  bloodbank-network:
    driver: bridge
"@ | Set-Content $compose

    Write-Success "Docker Compose creado."
}


function Create-BackendEnvironment {

    Write-Info "Creando appsettings.Development.json..."

    $settings = "$Backend\API\appsettings.Development.json"

@"
{
    "ConnectionStrings": {
        "DefaultConnection": "Host=postgres;Port=5432;Database=BloodBankDB;Username=postgres;Password=$DbPassword"
    },
    "Jwt": {
        "Key": "$JwtSecret",
        "Issuer": "BloodBankAPI",
        "Audience": "BloodBankClient",
        "ExpirationMinutes": 60,
        "RefreshExpirationDays": 7
    },
    "Serilog": {
        "MinimumLevel": "Information"
    }
}
"@ | Set-Content $settings

    Write-Success "appsettings.Development.json creado."
}


# =========================================================
# .gitignore
# =========================================================

function Create-GitIgnore {

    $file = "$Root\.gitignore"

@"
# Build
bin/
obj/
dist/

# Visual Studio
.vs/
*.user
*.suo

# VS Code
.vscode/

# Node
node_modules/

# Environment — NUNCA subir al repositorio
.env
*.local
**/appsettings.Development.json

# Logs
logs/
*.log

# Database
*.bak
*.mdf
*.ldf

# Docker volumes
docker-data/

# OS
.DS_Store
Thumbs.db
"@ | Set-Content $file

    Write-Success ".gitignore creado."
}


# =========================================================
# .editorconfig
# =========================================================

function Create-EditorConfig {

    $file = "$Root\.editorconfig"

@"
root = true

[*]
charset                  = utf-8
end_of_line              = crlf
insert_final_newline     = true
indent_style             = space
indent_size              = 4

[*.cs]
dotnet_sort_system_directives_first = true
csharp_new_line_before_open_brace   = all

[*.{ts,tsx,js,jsx}]
indent_size = 2

[*.json]
indent_size = 2

[*.yml]
indent_size = 2
"@ | Set-Content $file

    Write-Success ".editorconfig creado."
}


# =========================================================
# README
# =========================================================

function Create-Readme {

    $file = "$Root\README.md"

@"
# Blood Bank Management System

Sistema de gestión de banco de sangre con arquitectura empresarial.

## Stack

| Capa       | Tecnología                          |
|------------|-------------------------------------|
| Backend    | ASP.NET Core .NET 10                |
| Frontend   | React 19 + Vite + TypeScript        |
| Base datos | PostgreSQL 17                       |
| ORM        | Entity Framework Core               |
| Patrón     | Clean Architecture + DDD + CQRS     |
| Mensajería | MediatR                             |
| Auth       | JWT + Refresh Token                 |
| Logs       | Serilog                             |
| Contenedor | Docker + Docker Compose             |

## Estructura

\`\`\`
Backend/
  Domain/
    Aggregates/
      UserAggregate/
      DonorAggregate/
      DonationAggregate/
      BloodUnitAggregate/
    Enums/
    Events/
    ValueObjects/
  Application/
    Features/
      Auth/
      Donors/
      Donations/
      BloodUnits/
      Inventory/
      Reports/
    Common/
      Interfaces/
      Behaviors/
  Infrastructure/
    Persistence/
    Repositories/
    Security/
  API/
    Controllers/
    Middleware/
Frontend/
  src/
    pages/
    components/
    store/
    hooks/
    types/
\`\`\`

## Inicio rápido

\`\`\`powershell
# Con valores por defecto
./BloodBankManagementSystem.ps1

# Con credenciales personalizadas
./BloodBankManagementSystem.ps1 -DbPassword "MiPassword" -JwtSecret "MiClaveJWT32chars"
\`\`\`

## Docker

\`\`\`bash
cd Docker
docker compose up -d
\`\`\`

Servicios:
- API:     http://localhost:5000
- Web:     http://localhost:3000
- PgAdmin: http://localhost:5050

## Advertencia de seguridad

Los archivos `.env` y `appsettings.Development.json` están en `.gitignore`.
**Nunca subas credenciales al repositorio.**
"@ | Set-Content $file

    Write-Success "README.md creado."
}


# =========================================================
# Proceso principal — orden correcto de ejecución
# =========================================================

function Start-Setup {

    Write-Host ""
    Write-Host "  Blood Bank Management System — Setup" -ForegroundColor White
    Write-Host "  Proyecto: $ProjectName" -ForegroundColor Gray
    Write-Host ""

    Test-Prerequisites

    Initialize-Folders

    Create-Solution

    Create-BackendProjects

    Create-CleanArchitectureFolders

    Configure-ProjectReferences

    Install-NuGetPackages

    Create-DomainFiles

    Create-ApplicationFiles

    if ($UseFrontend)
    {
        Create-Frontend
        Create-FrontendFolders
        Create-FrontendFiles
        Create-FrontendEnvironment
    }

    if ($UseDocker)
    {
        Create-ApiDockerfile
        Create-BackendEnvironment
        Create-PostgreSQLConfiguration
        Create-PgAdminConfiguration
        Create-DockerCompose

        if ($UseFrontend)
        {
            Create-FrontendDockerfile
        }
    }

    Create-ConfigurationFiles

    Write-Host ""
    Write-Host "  =============================================" -ForegroundColor Green
    Write-Host "  Proyecto generado correctamente" -ForegroundColor Green
    Write-Host "  Ruta: $Root" -ForegroundColor Green
    Write-Host "  =============================================" -ForegroundColor Green
    Write-Host ""

    if ($DbPassword -eq "CHANGE_ME_IN_PRODUCTION")
    {
        Write-Host "  [WARN] Usando contraseña por defecto." -ForegroundColor Yellow
        Write-Host "         Ejecuta con -DbPassword 'TuPassword' antes de desplegar." -ForegroundColor Yellow
        Write-Host ""
    }
}


# =========================================================
# EJECUCIÓN
# =========================================================

Start-Setup
