<#
===========================================================
 Gmint Blood Bank — Starter Kit (v2)
 Setup Script

 Plataforma:
   - .NET 10
   - React 19 + TypeScript
   - PostgreSQL 17
   - Docker
   - Clean Architecture + DDD + CQRS (propio, sin MediatR/AutoMapper)
   - Multi-tenant (multi-sede)
   - Licenciamiento y Feature Flags

 Uso:
   ./GmintBloodBank.ps1
   ./GmintBloodBank.ps1 -ProjectName "MiGmint" -DbPassword "S3cur3!"
   ./GmintBloodBank.ps1 -UseDocker $false
===========================================================
#>


param(
    [string]$ProjectName = "GmintBloodBank",
    [bool]$UseDocker     = $true,
    [bool]$UseFrontend   = $true,

    # Credenciales parametrizadas — CAMBIAR antes de producción
    [string]$DbPassword      = "CHANGE_ME_IN_PRODUCTION",
    [string]$PgAdminEmail    = "admin@gmintbloodbank.com",
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

    dotnet new sln -n GmintBloodBank

    Write-Success "Solución GmintBloodBank.sln creada."

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
    dotnet new xunit    -n UnitTests
    dotnet new xunit    -n IntegrationTests

    Write-Success "Proyectos creados."

    Write-Info "Agregando proyectos a la solución..."

    dotnet sln GmintBloodBank.sln add API/API.csproj
    dotnet sln GmintBloodBank.sln add Domain/Domain.csproj
    dotnet sln GmintBloodBank.sln add Application/Application.csproj
    dotnet sln GmintBloodBank.sln add Infrastructure/Infrastructure.csproj
    dotnet sln GmintBloodBank.sln add UnitTests/UnitTests.csproj
    dotnet sln GmintBloodBank.sln add IntegrationTests/IntegrationTests.csproj

    Write-Success "Proyectos registrados en la solución."

    Pop-Location
}


# =========================================================
# Estructura interna Clean Architecture (según esquema solicitado)
# =========================================================

function Create-CleanArchitectureFolders {

    Write-Section "Estructura Clean Architecture"

    $folders = @(

        # ---------------------------------------------------
        # API
        # ---------------------------------------------------
        "$Backend\API\Controllers",
        "$Backend\API\Middleware",
        "$Backend\API\Filters",
        "$Backend\API\Extensions",

        # ---------------------------------------------------
        # Domain
        # ---------------------------------------------------
        "$Backend\Domain\Entities",
        "$Backend\Domain\Enums",
        "$Backend\Domain\ValueObjects",
        "$Backend\Domain\Exceptions",

        # Entidades organizadas por contexto dentro de Entities
        "$Backend\Domain\Entities\Security",
        "$Backend\Domain\Entities\Tenancy",
        "$Backend\Domain\Entities\Licensing",
        "$Backend\Domain\Entities\Donors",
        "$Backend\Domain\Entities\Donations",
        "$Backend\Domain\Entities\BloodUnits",
        "$Backend\Domain\Entities\Inventory",
        "$Backend\Domain\Entities\Notifications",
        "$Backend\Domain\Entities\Common",

        # ---------------------------------------------------
        # Application
        # ---------------------------------------------------
        "$Backend\Application\Common",
        "$Backend\Application\Common\Behaviors",
        "$Backend\Application\Common\Exceptions",
        "$Backend\Application\Common\Models",
        "$Backend\Application\Common\Mappings",
        "$Backend\Application\Common\CQRS",

        "$Backend\Application\DTOs",
        "$Backend\Application\DTOs\Auth",
        "$Backend\Application\DTOs\Donors",
        "$Backend\Application\DTOs\Donations",
        "$Backend\Application\DTOs\BloodUnits",
        "$Backend\Application\DTOs\Inventory",
        "$Backend\Application\DTOs\Reports",
        "$Backend\Application\DTOs\Tenancy",
        "$Backend\Application\DTOs\Licensing",

        "$Backend\Application\Interfaces",
        "$Backend\Application\Interfaces\Persistence",
        "$Backend\Application\Interfaces\Services",
        "$Backend\Application\Interfaces\Licensing",
        "$Backend\Application\Interfaces\Tenancy",

        # Features (CQRS propio: Commands / Queries / Handlers / Validators)
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

        "$Backend\Application\Features\Tenancy\Commands",
        "$Backend\Application\Features\Tenancy\Queries",

        "$Backend\Application\Features\Licensing\Commands",
        "$Backend\Application\Features\Licensing\Queries",

        # ---------------------------------------------------
        # Infrastructure
        # ---------------------------------------------------
        "$Backend\Infrastructure\Persistence",
        "$Backend\Infrastructure\Persistence\Migrations",
        "$Backend\Infrastructure\Persistence\Interceptors",
        "$Backend\Infrastructure\Persistence\Seeds",

        "$Backend\Infrastructure\Configurations",
        "$Backend\Infrastructure\Configurations\Security",
        "$Backend\Infrastructure\Configurations\Tenancy",
        "$Backend\Infrastructure\Configurations\Licensing",
        "$Backend\Infrastructure\Configurations\Donors",
        "$Backend\Infrastructure\Configurations\Donations",
        "$Backend\Infrastructure\Configurations\BloodUnits",
        "$Backend\Infrastructure\Configurations\Inventory",

        "$Backend\Infrastructure\Repositories",
        "$Backend\Infrastructure\Services",
        "$Backend\Infrastructure\Security"
    )

    foreach ($folder in $folders)
    {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
    }

    Write-Success "Estructura Clean Architecture + DDD creada según esquema."
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

    # UnitTests → Application + Domain
    dotnet add UnitTests/UnitTests.csproj reference Application/Application.csproj
    dotnet add UnitTests/UnitTests.csproj reference Domain/Domain.csproj

    # IntegrationTests → API + Infrastructure
    dotnet add IntegrationTests/IntegrationTests.csproj reference API/API.csproj
    dotnet add IntegrationTests/IntegrationTests.csproj reference Infrastructure/Infrastructure.csproj

    Write-Success "Referencias configuradas. Capas: Domain <- Application <- Infrastructure <- API"

    Pop-Location
}


# =========================================================
# Instalar paquetes NuGet por capa
# (Sin MediatR / sin AutoMapper — CQRS y mapeo propios)
# =========================================================

function Install-NuGetPackages {

    Write-Section "Paquetes NuGet"

    Push-Location $Backend

    # --- Domain: sin dependencias externas ---

    # --- Application: validación + abstracciones propias de CQRS ---
    $appPackages = @(
        "FluentValidation",
        "FluentValidation.DependencyInjectionExtensions"
    )

    foreach ($pkg in $appPackages)
    {
        dotnet add Application/Application.csproj package $pkg
    }

    Write-Success "Paquetes Application instalados (sin MediatR)."

    # --- Infrastructure: EF Core + Postgres + Serilog ---
    $infraPackages = @(
        "Microsoft.EntityFrameworkCore",
        "Microsoft.EntityFrameworkCore.Design",
        "Npgsql.EntityFrameworkCore.PostgreSQL",
        "Serilog.AspNetCore",
        "Serilog.Sinks.Console",
        "Serilog.Sinks.File"
    )

    foreach ($pkg in $infraPackages)
    {
        dotnet add Infrastructure/Infrastructure.csproj package $pkg
    }

    Write-Success "Paquetes Infrastructure instalados."

    # --- API: capa de presentación (sin AutoMapper) ---
    $apiPackages = @(
        "Microsoft.AspNetCore.Authentication.JwtBearer",
        "Swashbuckle.AspNetCore"
    )

    foreach ($pkg in $apiPackages)
    {
        dotnet add API/API.csproj package $pkg
    }

    Write-Success "Paquetes API instalados (sin AutoMapper)."

    # --- UnitTests ---
    dotnet add UnitTests/UnitTests.csproj package FluentAssertions
    dotnet add UnitTests/UnitTests.csproj package Moq

    # --- IntegrationTests ---
    dotnet add IntegrationTests/IntegrationTests.csproj package Microsoft.AspNetCore.Mvc.Testing
    dotnet add IntegrationTests/IntegrationTests.csproj package FluentAssertions
    dotnet add IntegrationTests/IntegrationTests.csproj package Testcontainers.PostgreSql

    Write-Success "Paquetes de pruebas instalados."

    Pop-Location
}


# =========================================================
# Helper: crear archivo .cs vacío (o con contenido)
# =========================================================

function New-CsFile {
    param($Path, $Content = $null)

    if (!(Test-Path $Path))
    {
        New-Item -ItemType File -Path $Path -Force | Out-Null
    }

    if ($null -ne $Content)
    {
        Set-Content -Path $Path -Value $Content
    }
}


# =========================================================
# Capa Domain — Entidades basadas en GmintBloodBank_v2.sql
# =========================================================

function Create-DomainFiles {

    Write-Section "Capa Domain"

    # --- Common (clases base de DDD) ---
    @("Entity", "AggregateRoot", "AuditableEntity", "ITenantEntity", "DomainEvent", "IDomainEvent") | ForEach-Object {
        New-CsFile "$Backend\Domain\Entities\Common\$_.cs"
    }

    # --- Security ---
    @("Role", "User", "RefreshToken", "AuditLog") | ForEach-Object {
        New-CsFile "$Backend\Domain\Entities\Security\$_.cs"
    }

    # --- Tenancy (Multi-sede / Multi-tenant) ---
    @("Tenant", "BloodBank", "Address", "Staff", "StaffCategory") | ForEach-Object {
        New-CsFile "$Backend\Domain\Entities\Tenancy\$_.cs"
    }

    # --- Licensing (Licenciamiento y Feature Flags) ---
    @("License", "LicensePlan", "FeatureFlag", "TenantFeature", "Subscription") | ForEach-Object {
        New-CsFile "$Backend\Domain\Entities\Licensing\$_.cs"
    }

    # --- Donors ---
    @("Donor", "MedicalCondition", "DonorMedicalCondition", "Gender", "BloodType") | ForEach-Object {
        New-CsFile "$Backend\Domain\Entities\Donors\$_.cs"
    }

    # --- Donations ---
    @("DonationAppointment", "DonationEvaluation", "Donation", "DonationStatus") | ForEach-Object {
        New-CsFile "$Backend\Domain\Entities\Donations\$_.cs"
    }

    # --- BloodUnits (trazabilidad) ---
    @("BloodUnit", "BloodUnitStatus", "BloodComponent", "BloodScreening") | ForEach-Object {
        New-CsFile "$Backend\Domain\Entities\BloodUnits\$_.cs"
    }

    # --- Inventory ---
    @("InventoryMovement", "InventoryMovementType", "StorageLocation") | ForEach-Object {
        New-CsFile "$Backend\Domain\Entities\Inventory\$_.cs"
    }

    # --- Notifications ---
    @("Notification", "NotificationType") | ForEach-Object {
        New-CsFile "$Backend\Domain\Entities\Notifications\$_.cs"
    }

    # --- Enums ---
    @(
        "BloodTypeCode",
        "RhFactor",
        "DonationStatusCode",
        "BloodUnitStatusCode",
        "InventoryMovementTypeCode",
        "NotificationTypeCode",
        "ScreeningResult",
        "AuditAction",
        "LicenseStatus",
        "FeatureFlagScope"
    ) | ForEach-Object {
        New-CsFile "$Backend\Domain\Enums\$_.cs"
    }

    # --- Value Objects ---
    @("PersonName", "AddressVO", "BloodTypeVO", "QrCode", "DateRange", "Email") | ForEach-Object {
        New-CsFile "$Backend\Domain\ValueObjects\$_.cs"
    }

    # --- Exceptions ---
    @(
        "DomainException",
        "InvalidBloodTypeException",
        "BloodUnitExpiredException",
        "DonorNotEligibleException",
        "TenantMismatchException",
        "FeatureNotLicensedException"
    ) | ForEach-Object {
        New-CsFile "$Backend\Domain\Exceptions\$_.cs"
    }

    Write-Success "Entidades de dominio generadas a partir del esquema SQL (incluye Tenancy y Licensing)."
}


# =========================================================
# Capa Application — CQRS propio (sin MediatR/AutoMapper)
# =========================================================

function Create-ApplicationFiles {

    Write-Section "Capa Application"

    # -----------------------------------------------------
    # CQRS propio: abstracciones base
    # -----------------------------------------------------
    $cqrsCommandBus = @"
namespace Application.Common.CQRS;

/// <summary>
/// Marca un comando que produce una respuesta de tipo TResponse.
/// Reemplaza IRequest&lt;T&gt; de MediatR con una abstracción propia.
/// </summary>
public interface ICommand<TResponse> { }

/// <summary>
/// Manejador de un comando. Reemplaza IRequestHandler de MediatR.
/// </summary>
public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Despachador de comandos. Resuelve el handler correspondiente
/// desde el contenedor de DI y lo ejecuta.
/// </summary>
public interface ICommandDispatcher
{
    Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
}
"@
    New-CsFile "$Backend\Application\Common\CQRS\ICommand.cs" $cqrsCommandBus

    $cqrsQueryBus = @"
namespace Application.Common.CQRS;

/// <summary>
/// Marca una consulta que produce una respuesta de tipo TResponse.
/// </summary>
public interface IQuery<TResponse> { }

/// <summary>
/// Manejador de una consulta.
/// </summary>
public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Despachador de consultas. Resuelve el handler correspondiente
/// desde el contenedor de DI y lo ejecuta.
/// </summary>
public interface IQueryDispatcher
{
    Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
"@
    New-CsFile "$Backend\Application\Common\CQRS\IQuery.cs" $cqrsQueryBus

    $cqrsDispatcherImpl = @"
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.CQRS;

/// <summary>
/// Implementación propia del despachador de comandos basada en
/// resolución dinámica de servicios (reemplaza el pipeline de MediatR).
/// </summary>
public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));

        var handler = _serviceProvider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod("HandleAsync")
            ?? throw new InvalidOperationException($"No se encontró HandleAsync para {handlerType.Name}");

        var result = method.Invoke(handler, new object[] { command, cancellationToken })
            ?? throw new InvalidOperationException("El handler retornó null.");

        return (Task<TResponse>)result;
    }
}
"@
    New-CsFile "$Backend\Application\Common\CQRS\CommandDispatcher.cs" $cqrsDispatcherImpl

    $cqrsQueryDispatcherImpl = @"
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.CQRS;

/// <summary>
/// Implementación propia del despachador de consultas.
/// </summary>
public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));

        var handler = _serviceProvider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod("HandleAsync")
            ?? throw new InvalidOperationException($"No se encontró HandleAsync para {handlerType.Name}");

        var result = method.Invoke(handler, new object[] { query, cancellationToken })
            ?? throw new InvalidOperationException("El handler retornó null.");

        return (Task<TResponse>)result;
    }
}
"@
    New-CsFile "$Backend\Application\Common\CQRS\QueryDispatcher.cs" $cqrsQueryDispatcherImpl

    # -----------------------------------------------------
    # Mapeo manual (sin AutoMapper)
    # -----------------------------------------------------
    $manualMapper = @"
namespace Application.Common.Mappings;

/// <summary>
/// Contrato de mapeo manual entre una entidad de dominio y un DTO.
/// Reemplaza los perfiles de AutoMapper con código explícito y testeable.
/// </summary>
public interface IMapper<TEntity, TDto>
{
    TDto ToDto(TEntity entity);

    TEntity ToEntity(TDto dto);
}

/// <summary>
/// Contrato de mapeo de solo lectura (proyección hacia DTO).
/// Útil para Queries donde no se requiere reconstrucción de la entidad.
/// </summary>
public interface IReadOnlyMapper<TEntity, TDto>
{
    TDto ToDto(TEntity entity);
}
"@
    New-CsFile "$Backend\Application\Common\Mappings\IMapper.cs" $manualMapper

    $mapperExample = @"
using Application.DTOs.Donors;
using Domain.Entities.Donors;

namespace Application.Common.Mappings;

/// <summary>
/// Ejemplo de mapeo manual Donor &lt;-&gt; DonorDto.
/// Cada feature debe implementar su propio mapper siguiendo este patrón,
/// sin depender de AutoMapper ni librerías de reflexión externas.
/// </summary>
public sealed class DonorMapper : IMapper<Donor, DonorDto>
{
    public DonorDto ToDto(Donor entity)
    {
        return new DonorDto
        {
            Id          = entity.Id,
            FirstName   = entity.FirstName,
            LastName    = entity.LastName,
            DocumentId  = entity.DocumentId,
            Email       = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            BloodTypeId = entity.BloodTypeId,
            IsEligible  = entity.IsEligible
        };
    }

    public Donor ToEntity(DonorDto dto)
    {
        return new Donor
        {
            Id          = dto.Id,
            FirstName   = dto.FirstName,
            LastName    = dto.LastName,
            DocumentId  = dto.DocumentId,
            Email       = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            BloodTypeId = dto.BloodTypeId,
            IsEligible  = dto.IsEligible
        };
    }
}
"@
    New-CsFile "$Backend\Application\Common\Mappings\DonorMapper.cs" $mapperExample

    # -----------------------------------------------------
    # Interfaces comunes (persistencia / servicios / tenancy / licensing)
    # -----------------------------------------------------
    @("IRepository", "IUnitOfWork") | ForEach-Object {
        New-CsFile "$Backend\Application\Interfaces\Persistence\$_.cs"
    }

    @("ICurrentUserService", "IJwtService", "IAuditService", "IDateTimeProvider", "IQrCodeService") | ForEach-Object {
        New-CsFile "$Backend\Application\Interfaces\Services\$_.cs"
    }

    # Multi-tenant
    $tenantContext = @"
namespace Application.Interfaces.Tenancy;

/// <summary>
/// Contexto del tenant (sede / banco de sangre) actual de la solicitud.
/// Implementado en Infrastructure a partir del JWT, header o subdominio.
/// </summary>
public interface ITenantContext
{
    Guid? TenantId { get; }

    string? TenantCode { get; }

    bool HasTenant { get; }

    void SetTenant(Guid tenantId, string tenantCode);
}
"@
    New-CsFile "$Backend\Application\Interfaces\Tenancy\ITenantContext.cs" $tenantContext

    $tenantProvider = @"
namespace Application.Interfaces.Tenancy;

/// <summary>
/// Resuelve el tenant activo a partir de la solicitud HTTP
/// (claim del JWT, header X-Tenant-Id o subdominio).
/// </summary>
public interface ITenantProvider
{
    Task<Guid?> ResolveTenantIdAsync(CancellationToken cancellationToken = default);
}
"@
    New-CsFile "$Backend\Application\Interfaces\Tenancy\ITenantProvider.cs" $tenantProvider

    # Licensing / Feature Flags
    $licenseService = @"
namespace Application.Interfaces.Licensing;

/// <summary>
/// Servicio de licenciamiento. Valida el estado de la licencia
/// del tenant actual (activa, expirada, suspendida, límites de uso).
/// </summary>
public interface ILicenseService
{
    Task<bool> IsLicenseActiveAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<bool> HasReachedUsageLimitAsync(Guid tenantId, string resourceKey, CancellationToken cancellationToken = default);
}
"@
    New-CsFile "$Backend\Application\Interfaces\Licensing\ILicenseService.cs" $licenseService

    $featureFlagService = @"
namespace Application.Interfaces.Licensing;

/// <summary>
/// Servicio de Feature Flags. Determina si una funcionalidad
/// está habilitada de forma global o para un tenant específico.
/// </summary>
public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(string featureKey, Guid? tenantId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, bool>> GetAllFlagsAsync(Guid? tenantId = null, CancellationToken cancellationToken = default);
}
"@
    New-CsFile "$Backend\Application\Interfaces\Licensing\IFeatureFlagService.cs" $featureFlagService

    # -----------------------------------------------------
    # Modelos comunes
    # -----------------------------------------------------
    @("Result", "PagedResult", "PaginationQuery") | ForEach-Object {
        New-CsFile "$Backend\Application\Common\Models\$_.cs"
    }

    # -----------------------------------------------------
    # Behaviors (decoradores propios, sin pipeline de MediatR)
    # -----------------------------------------------------
    @("ValidationDecorator", "LoggingDecorator", "TransactionDecorator", "TenantGuardDecorator", "LicenseGuardDecorator") | ForEach-Object {
        New-CsFile "$Backend\Application\Common\Behaviors\$_.cs"
    }

    # Excepciones de Application
    @("ValidationException", "NotFoundException", "ForbiddenAccessException", "LicenseExpiredException") | ForEach-Object {
        New-CsFile "$Backend\Application\Common\Exceptions\$_.cs"
    }

    # -----------------------------------------------------
    # DTOs
    # -----------------------------------------------------
    @("LoginRequestDto", "LoginResponseDto", "RefreshTokenRequestDto", "UserDto") | ForEach-Object {
        New-CsFile "$Backend\Application\DTOs\Auth\$_.cs"
    }

    @("DonorDto", "DonorMedicalConditionDto") | ForEach-Object {
        New-CsFile "$Backend\Application\DTOs\Donors\$_.cs"
    }

    @("DonationAppointmentDto", "DonationEvaluationDto", "DonationDto") | ForEach-Object {
        New-CsFile "$Backend\Application\DTOs\Donations\$_.cs"
    }

    @("BloodUnitDto", "BloodScreeningDto") | ForEach-Object {
        New-CsFile "$Backend\Application\DTOs\BloodUnits\$_.cs"
    }

    @("InventoryMovementDto", "InventorySummaryDto") | ForEach-Object {
        New-CsFile "$Backend\Application\DTOs\Inventory\$_.cs"
    }

    @("BloodStockSummaryDto", "DonationsReportDto", "ExpirationReportDto") | ForEach-Object {
        New-CsFile "$Backend\Application\DTOs\Reports\$_.cs"
    }

    @("TenantDto", "BloodBankDto", "CreateTenantDto") | ForEach-Object {
        New-CsFile "$Backend\Application\DTOs\Tenancy\$_.cs"
    }

    @("LicenseDto", "FeatureFlagDto", "SubscriptionDto") | ForEach-Object {
        New-CsFile "$Backend\Application\DTOs\Licensing\$_.cs"
    }

    # -----------------------------------------------------
    # Features — Auth
    # -----------------------------------------------------
    New-CsFile "$Backend\Application\Features\Auth\Commands\LoginCommand.cs"
    New-CsFile "$Backend\Application\Features\Auth\Commands\LoginCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\Auth\Commands\RefreshTokenCommand.cs"
    New-CsFile "$Backend\Application\Features\Auth\Commands\RefreshTokenCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\Auth\Queries\GetCurrentUserQuery.cs"
    New-CsFile "$Backend\Application\Features\Auth\Queries\GetCurrentUserQueryHandler.cs"

    # -----------------------------------------------------
    # Features — Donors
    # -----------------------------------------------------
    New-CsFile "$Backend\Application\Features\Donors\Commands\CreateDonorCommand.cs"
    New-CsFile "$Backend\Application\Features\Donors\Commands\CreateDonorCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\Donors\Commands\CreateDonorCommandValidator.cs"
    New-CsFile "$Backend\Application\Features\Donors\Commands\UpdateDonorCommand.cs"
    New-CsFile "$Backend\Application\Features\Donors\Commands\UpdateDonorCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\Donors\Queries\GetDonorsQuery.cs"
    New-CsFile "$Backend\Application\Features\Donors\Queries\GetDonorsQueryHandler.cs"
    New-CsFile "$Backend\Application\Features\Donors\Queries\GetDonorByIdQuery.cs"
    New-CsFile "$Backend\Application\Features\Donors\Queries\GetDonorByIdQueryHandler.cs"

    # -----------------------------------------------------
    # Features — Donations
    # -----------------------------------------------------
    New-CsFile "$Backend\Application\Features\Donations\Commands\CreateDonationAppointmentCommand.cs"
    New-CsFile "$Backend\Application\Features\Donations\Commands\CreateDonationAppointmentCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\Donations\Commands\RegisterEvaluationCommand.cs"
    New-CsFile "$Backend\Application\Features\Donations\Commands\RegisterEvaluationCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\Donations\Commands\RegisterDonationCommand.cs"
    New-CsFile "$Backend\Application\Features\Donations\Commands\RegisterDonationCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\Donations\Queries\GetDonationsQuery.cs"
    New-CsFile "$Backend\Application\Features\Donations\Queries\GetDonationsQueryHandler.cs"

    # -----------------------------------------------------
    # Features — BloodUnits
    # -----------------------------------------------------
    New-CsFile "$Backend\Application\Features\BloodUnits\Commands\RegisterBloodUnitCommand.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Commands\RegisterBloodUnitCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Commands\RegisterScreeningCommand.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Commands\RegisterScreeningCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Commands\ReleaseBloodUnitCommand.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Commands\ReleaseBloodUnitCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Commands\DiscardBloodUnitCommand.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Commands\DiscardBloodUnitCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Queries\GetBloodUnitByCodeQuery.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Queries\GetBloodUnitByCodeQueryHandler.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Queries\GetAvailableInventoryQuery.cs"
    New-CsFile "$Backend\Application\Features\BloodUnits\Queries\GetAvailableInventoryQueryHandler.cs"

    # -----------------------------------------------------
    # Features — Inventory
    # -----------------------------------------------------
    New-CsFile "$Backend\Application\Features\Inventory\Commands\RegisterMovementCommand.cs"
    New-CsFile "$Backend\Application\Features\Inventory\Commands\RegisterMovementCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\Inventory\Queries\GetInventoryQuery.cs"
    New-CsFile "$Backend\Application\Features\Inventory\Queries\GetInventoryQueryHandler.cs"
    New-CsFile "$Backend\Application\Features\Inventory\Queries\GetExpiringUnitsQuery.cs"
    New-CsFile "$Backend\Application\Features\Inventory\Queries\GetExpiringUnitsQueryHandler.cs"

    # -----------------------------------------------------
    # Features — Reports
    # -----------------------------------------------------
    New-CsFile "$Backend\Application\Features\Reports\Queries\GetBloodStockSummaryQuery.cs"
    New-CsFile "$Backend\Application\Features\Reports\Queries\GetBloodStockSummaryQueryHandler.cs"
    New-CsFile "$Backend\Application\Features\Reports\Queries\GetDonationsReportQuery.cs"
    New-CsFile "$Backend\Application\Features\Reports\Queries\GetDonationsReportQueryHandler.cs"

    # -----------------------------------------------------
    # Features — Tenancy (Multi-sede / Multi-tenant)
    # -----------------------------------------------------
    New-CsFile "$Backend\Application\Features\Tenancy\Commands\CreateTenantCommand.cs"
    New-CsFile "$Backend\Application\Features\Tenancy\Commands\CreateTenantCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\Tenancy\Commands\UpdateTenantCommand.cs"
    New-CsFile "$Backend\Application\Features\Tenancy\Commands\UpdateTenantCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\Tenancy\Queries\GetTenantsQuery.cs"
    New-CsFile "$Backend\Application\Features\Tenancy\Queries\GetTenantsQueryHandler.cs"
    New-CsFile "$Backend\Application\Features\Tenancy\Queries\GetTenantByIdQuery.cs"
    New-CsFile "$Backend\Application\Features\Tenancy\Queries\GetTenantByIdQueryHandler.cs"

    # -----------------------------------------------------
    # Features — Licensing (Licenciamiento y Feature Flags)
    # -----------------------------------------------------
    New-CsFile "$Backend\Application\Features\Licensing\Commands\AssignLicenseCommand.cs"
    New-CsFile "$Backend\Application\Features\Licensing\Commands\AssignLicenseCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\Licensing\Commands\ToggleFeatureFlagCommand.cs"
    New-CsFile "$Backend\Application\Features\Licensing\Commands\ToggleFeatureFlagCommandHandler.cs"
    New-CsFile "$Backend\Application\Features\Licensing\Queries\GetLicenseStatusQuery.cs"
    New-CsFile "$Backend\Application\Features\Licensing\Queries\GetLicenseStatusQueryHandler.cs"
    New-CsFile "$Backend\Application\Features\Licensing\Queries\GetTenantFeatureFlagsQuery.cs"
    New-CsFile "$Backend\Application\Features\Licensing\Queries\GetTenantFeatureFlagsQueryHandler.cs"

    Write-Success "Capa Application creada con CQRS propio (Commands/Queries/Handlers) y mapeo manual."
}


# =========================================================
# Capa Infrastructure
# =========================================================

function Create-InfrastructureFiles {

    Write-Section "Capa Infrastructure"

    # Persistence
    @("ApplicationDbContext", "DbContextFactory") | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Persistence\$_.cs"
    }

    @("SoftDeleteInterceptor", "AuditableEntityInterceptor", "TenantSaveChangesInterceptor") | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Persistence\Interceptors\$_.cs"
    }

    @("InitialSeed", "CatalogSeed", "LicensingSeed", "TenancySeed") | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Persistence\Seeds\$_.cs"
    }

    # Configurations (EF Core Fluent API) — un archivo por entidad principal
    @("RoleConfiguration", "UserConfiguration", "RefreshTokenConfiguration", "AuditLogConfiguration") | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Configurations\Security\$_.cs"
    }

    @("TenantConfiguration", "BloodBankConfiguration", "AddressConfiguration", "StaffConfiguration", "StaffCategoryConfiguration") | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Configurations\Tenancy\$_.cs"
    }

    @("LicenseConfiguration", "LicensePlanConfiguration", "FeatureFlagConfiguration", "TenantFeatureConfiguration", "SubscriptionConfiguration") | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Configurations\Licensing\$_.cs"
    }

    @("DonorConfiguration", "MedicalConditionConfiguration", "DonorMedicalConditionConfiguration", "GenderConfiguration", "BloodTypeConfiguration") | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Configurations\Donors\$_.cs"
    }

    @("DonationAppointmentConfiguration", "DonationEvaluationConfiguration", "DonationConfiguration", "DonationStatusConfiguration") | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Configurations\Donations\$_.cs"
    }

    @("BloodUnitConfiguration", "BloodUnitStatusConfiguration", "BloodComponentConfiguration", "BloodScreeningConfiguration") | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Configurations\BloodUnits\$_.cs"
    }

    @("InventoryMovementConfiguration", "InventoryMovementTypeConfiguration", "StorageLocationConfiguration") | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Configurations\Inventory\$_.cs"
    }

    # Repositories (implementación de IRepository / IUnitOfWork)
    @(
        "GenericRepository",
        "UnitOfWork",
        "DonorRepository",
        "DonationRepository",
        "BloodUnitRepository",
        "InventoryRepository",
        "TenantRepository",
        "LicenseRepository",
        "FeatureFlagRepository"
    ) | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Repositories\$_.cs"
    }

    # Services
    @(
        "DateTimeProvider",
        "AuditService",
        "QrCodeService",
        "CurrentUserService"
    ) | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Services\$_.cs"
    }

    # Multi-tenant: implementación de contexto y proveedor de tenant
    $tenantContextImpl = @"
using Application.Interfaces.Tenancy;

namespace Infrastructure.Services;

/// <summary>
/// Implementación de ITenantContext almacenada con duración de la solicitud HTTP
/// (registrada como Scoped en DependencyInjection).
/// </summary>
public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }

    public string? TenantCode { get; private set; }

    public bool HasTenant => TenantId.HasValue;

    public void SetTenant(Guid tenantId, string tenantCode)
    {
        TenantId   = tenantId;
        TenantCode = tenantCode;
    }
}
"@
    New-CsFile "$Backend\Infrastructure\Services\TenantContext.cs" $tenantContextImpl

    $tenantProviderImpl = @"
using Application.Interfaces.Tenancy;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

/// <summary>
/// Resuelve el TenantId desde el claim "tenant_id" del JWT
/// o desde el header "X-Tenant-Id" como respaldo.
/// </summary>
public sealed class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Guid?> ResolveTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var context = _httpContextAccessor.HttpContext;

        if (context is null)
        {
            return Task.FromResult<Guid?>(null);
        }

        var claim = context.User.FindFirst("tenant_id")?.Value;

        if (Guid.TryParse(claim, out var tenantIdFromClaim))
        {
            return Task.FromResult<Guid?>(tenantIdFromClaim);
        }

        var header = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();

        if (Guid.TryParse(header, out var tenantIdFromHeader))
        {
            return Task.FromResult<Guid?>(tenantIdFromHeader);
        }

        return Task.FromResult<Guid?>(null);
    }
}
"@
    New-CsFile "$Backend\Infrastructure\Services\TenantProvider.cs" $tenantProviderImpl

    # Licensing / Feature Flags
    $licenseServiceImpl = @"
using Application.Interfaces.Licensing;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

/// <summary>
/// Valida el estado de licencia de un tenant contra la tabla Licenses/Subscriptions.
/// </summary>
public sealed class LicenseService : ILicenseService
{
    private readonly ApplicationDbContext _context;

    public LicenseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsLicenseActiveAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        // TODO: validar contra Subscriptions/License (estado, fecha de expiración).
        return await Task.FromResult(true);
    }

    public async Task<bool> HasReachedUsageLimitAsync(Guid tenantId, string resourceKey, CancellationToken cancellationToken = default)
    {
        // TODO: validar límites del plan (ej. número de usuarios, sedes, unidades por mes).
        return await Task.FromResult(false);
    }
}
"@
    New-CsFile "$Backend\Infrastructure\Services\LicenseService.cs" $licenseServiceImpl

    $featureFlagServiceImpl = @"
using Application.Interfaces.Licensing;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

/// <summary>
/// Resuelve Feature Flags combinando configuración global (FeatureFlags)
/// con overrides por tenant (TenantFeatures).
/// </summary>
public sealed class FeatureFlagService : IFeatureFlagService
{
    private readonly ApplicationDbContext _context;

    public FeatureFlagService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsEnabledAsync(string featureKey, Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        // TODO: 1) buscar override en TenantFeatures para tenantId
        //       2) si no existe, usar el valor por defecto de FeatureFlags
        return await Task.FromResult(false);
    }

    public async Task<IReadOnlyDictionary<string, bool>> GetAllFlagsAsync(Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        // TODO: combinar FeatureFlags globales + TenantFeatures del tenant.
        return await Task.FromResult<IReadOnlyDictionary<string, bool>>(new Dictionary<string, bool>());
    }
}
"@
    New-CsFile "$Backend\Infrastructure\Services\FeatureFlagService.cs" $featureFlagServiceImpl

    # Security
    @("JwtTokenService", "PasswordHasher", "PermissionPolicyProvider") | ForEach-Object {
        New-CsFile "$Backend\Infrastructure\Security\$_.cs"
    }

    # DependencyInjection raíz de Infrastructure
    $infraDi = @"
using Application.Interfaces.Licensing;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Application.Interfaces.Tenancy;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpContextAccessor();

        // Multi-tenant
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantProvider, TenantProvider>();

        // Licensing / Feature Flags
        services.AddScoped<ILicenseService, LicenseService>();
        services.AddScoped<IFeatureFlagService, FeatureFlagService>();

        // Servicios generales
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IQrCodeService, QrCodeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtService, JwtTokenService>();

        // Persistencia
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
"@
    New-CsFile "$Backend\Infrastructure\DependencyInjection.cs" $infraDi

    Write-Success "Capa Infrastructure creada (incluye soporte multi-tenant y licensing)."
}


# =========================================================
# Capa API
# =========================================================

function Create-ApiFiles {

    Write-Section "Capa API"

    # Controllers
    @(
        "AuthController",
        "DonorsController",
        "DonationsController",
        "BloodUnitsController",
        "InventoryController",
        "ReportsController",
        "TenantsController",
        "LicensingController"
    ) | ForEach-Object {
        New-CsFile "$Backend\API\Controllers\$_.cs"
    }

    # Middleware
    @("ExceptionHandlingMiddleware", "TenantResolutionMiddleware", "LicenseValidationMiddleware", "RequestLoggingMiddleware") | ForEach-Object {
        New-CsFile "$Backend\API\Middleware\$_.cs"
    }

    # Filters
    @("ValidationFilter", "RequireFeatureFlagAttribute", "ApiExceptionFilter") | ForEach-Object {
        New-CsFile "$Backend\API\Filters\$_.cs"
    }

    # Extensions (registro de servicios CQRS, Swagger, JWT, etc.)
    $cqrsExtension = @"
using Application.Common.CQRS;

namespace API.Extensions;

/// <summary>
/// Registra el despachador de comandos/consultas propio y escanea
/// los ensamblados de Application para registrar todos los handlers
/// (ICommandHandler / IQueryHandler) automáticamente — sin MediatR.
/// </summary>
public static class CqrsServiceExtensions
{
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        var applicationAssembly = typeof(Application.AssemblyReference).Assembly;

        RegisterHandlers(services, applicationAssembly, typeof(ICommandHandler<,>));
        RegisterHandlers(services, applicationAssembly, typeof(IQueryHandler<,>));

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, System.Reflection.Assembly assembly, Type openGenericInterface)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface)
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var handler in handlerTypes)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }
    }
}
"@
    New-CsFile "$Backend\API\Extensions\CqrsServiceExtensions.cs" $cqrsExtension

    @("SwaggerServiceExtensions", "JwtServiceExtensions", "CorsServiceExtensions") | ForEach-Object {
        New-CsFile "$Backend\API\Extensions\$_.cs"
    }

    # Marcador de ensamblado para Application (usado por AddCqrs)
    New-CsFile "$Backend\Application\AssemblyReference.cs" @"
namespace Application;

/// <summary>
/// Clase marcadora utilizada para referenciar el ensamblado Application
/// en operaciones de reflexión (registro de handlers CQRS).
/// </summary>
public static class AssemblyReference
{
}
"@

    Write-Success "Capa API creada (Controllers, Middleware, Filters, Extensions)."
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
        "$Root\Frontend\src\pages\Tenants",
        "$Root\Frontend\src\pages\Licensing",

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
        "$Root\Frontend\src\store\tenantStore.ts",
        "$Root\Frontend\src\store\featureFlagStore.ts",

        "$Root\Frontend\src\hooks\useAuth.ts",
        "$Root\Frontend\src\hooks\useFeatureFlag.ts",

        "$Root\Frontend\src\types\User.ts",
        "$Root\Frontend\src\types\Donor.ts",
        "$Root\Frontend\src\types\Donation.ts",
        "$Root\Frontend\src\types\BloodUnit.ts",
        "$Root\Frontend\src\types\Tenant.ts",
        "$Root\Frontend\src\types\License.ts",

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
        "$Root\Frontend\src\pages\Reports\Reports.tsx",
        "$Root\Frontend\src\pages\Tenants\TenantList.tsx",
        "$Root\Frontend\src\pages\Licensing\LicensingDashboard.tsx"
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
VITE_APP_NAME=Gmint Blood Bank
VITE_VERSION=2.0.0
"@ | Set-Content $envFile

    Write-Success ".env Frontend creado."
}


# =========================================================
# Base de datos — copiar script SQL de referencia
# =========================================================

function Copy-DatabaseScript {

    Write-Section "Script de base de datos"

    $source = Join-Path (Get-Location) "GmintBloodBank_v2.sql"

    if (Test-Path $source)
    {
        Copy-Item $source -Destination "$Database\Scripts\GmintBloodBank_v2.sql" -Force
        Write-Success "Script SQL copiado a Database/Scripts."
    }
    else
    {
        Write-Host "[WARN] No se encontró GmintBloodBank_v2.sql en el directorio actual. Omitido." -ForegroundColor Yellow
    }
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
POSTGRES_DB=GmintBloodBankDB
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
    container_name: gmint-postgres
    restart: unless-stopped
    env_file:
      - ./PostgreSQL/.env
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - gmint-network

  pgadmin:
    image: dpage/pgadmin4
    container_name: gmint-pgadmin
    restart: unless-stopped
    env_file:
      - ./PgAdmin/.env
    ports:
      - "5050:80"
    depends_on:
      - postgres
    networks:
      - gmint-network

  api:
    build:
      context: ../Backend
      dockerfile: API/Dockerfile
    container_name: gmint-api
    ports:
      - "5000:8080"
    depends_on:
      - postgres
    networks:
      - gmint-network

  frontend:
    build:
      context: ../Frontend
    container_name: gmint-web
    ports:
      - "3000:80"
    depends_on:
      - api
    networks:
      - gmint-network

volumes:
  postgres_data:

networks:
  gmint-network:
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
        "DefaultConnection": "Host=postgres;Port=5432;Database=GmintBloodBankDB;Username=postgres;Password=$DbPassword"
    },
    "Jwt": {
        "Key": "$JwtSecret",
        "Issuer": "GmintBloodBankAPI",
        "Audience": "GmintBloodBankClient",
        "ExpirationMinutes": 60,
        "RefreshExpirationDays": 7
    },
    "Licensing": {
        "EnforceLicenseValidation": true,
        "GracePeriodDays": 7
    },
    "FeatureFlags": {
        "DefaultProvider": "Database"
    },
    "MultiTenant": {
        "Enabled": true,
        "ResolutionStrategy": "JwtClaim"
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
# Gmint Blood Bank

Sistema de gestión de banco de sangre con arquitectura empresarial,
soporte multi-sede (multi-tenant) y licenciamiento por feature flags.

## Stack

| Capa       | Tecnología                          |
|------------|---------------------------------------|
| Backend    | ASP.NET Core .NET 10                   |
| Frontend   | React 19 + Vite + TypeScript           |
| Base datos | PostgreSQL 17                          |
| ORM        | Entity Framework Core                  |
| Patrón     | Clean Architecture + DDD + CQRS propio |
| CQRS       | Implementación propia (sin MediatR)    |
| Mapeo      | Manual (sin AutoMapper)                |
| Auth       | JWT + Refresh Token                    |
| Multi-tenant | Resolución por claim JWT / header    |
| Licenciamiento | Feature Flags por plan / tenant    |
| Logs       | Serilog                                 |
| Contenedor | Docker + Docker Compose                |

## CQRS propio

En \`Application/Common/CQRS\` se definen las abstracciones \`ICommand\`,
\`IQuery\`, \`ICommandHandler\`, \`IQueryHandler\`, junto con
\`CommandDispatcher\` y \`QueryDispatcher\`, que reemplazan a MediatR
mediante resolución de handlers vía DI y reflexión controlada.

## Mapeo manual

En \`Application/Common/Mappings\` se define \`IMapper<TEntity, TDto>\`
e \`IReadOnlyMapper<TEntity, TDto>\`. Cada feature implementa su propio
mapper explícito (ver \`DonorMapper\` como referencia), sin AutoMapper.

## Multi-tenant (Multi-sede)

- \`ITenantContext\` / \`TenantContext\`: contexto del tenant activo durante la solicitud.
- \`ITenantProvider\` / \`TenantProvider\`: resuelve el TenantId desde el JWT o header \`X-Tenant-Id\`.
- \`TenantResolutionMiddleware\`: aplica el tenant al inicio del pipeline.
- Las entidades multi-tenant implementan \`ITenantEntity\` (incluye \`TenantId\`).

## Licenciamiento y Feature Flags

- \`ILicenseService\`: valida estado de licencia y límites de uso por tenant.
- \`IFeatureFlagService\`: resuelve flags globales y overrides por tenant.
- \`LicenseValidationMiddleware\` y \`RequireFeatureFlagAttribute\` controlan
  el acceso a endpoints según el plan/licencia del tenant.

## Estructura

\`\`\`
Backend/
  Domain/
    Entities/
      Security/
      Tenancy/
      Licensing/
      Donors/
      Donations/
      BloodUnits/
      Inventory/
      Notifications/
      Common/
    Enums/
    ValueObjects/
    Exceptions/
  Application/
    Common/
      CQRS/
      Mappings/
      Behaviors/
      Exceptions/
      Models/
    DTOs/
    Interfaces/
      Persistence/
      Services/
      Tenancy/
      Licensing/
    Features/
      Auth/
      Donors/
      Donations/
      BloodUnits/
      Inventory/
      Reports/
      Tenancy/
      Licensing/
  Infrastructure/
    Persistence/
    Configurations/
    Repositories/
    Services/
    Security/
  API/
    Controllers/
    Middleware/
    Filters/
    Extensions/
    Program.cs
Tests/
  UnitTests/
  IntegrationTests/
Frontend/
  src/
    pages/
    components/
    store/
    hooks/
    types/
Database/
  Scripts/
    GmintBloodBank_v2.sql
\`\`\`

## Inicio rápido

\`\`\`powershell
# Con valores por defecto
./GmintBloodBank.ps1

# Con credenciales personalizadas
./GmintBloodBank.ps1 -DbPassword "MiPassword" -JwtSecret "MiClaveJWT32chars"
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

Los archivos \`.env\` y \`appsettings.Development.json\` están en \`.gitignore\`.
**Nunca subas credenciales al repositorio.**
"@ | Set-Content $file

    Write-Success "README.md creado."
}


# =========================================================
# Proceso principal — orden correcto de ejecución
# =========================================================

function Start-Setup {

    Write-Host ""
    Write-Host "  Gmint Blood Bank — Setup v2" -ForegroundColor White
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

    Create-InfrastructureFiles

    Create-ApiFiles

    Copy-DatabaseScript

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