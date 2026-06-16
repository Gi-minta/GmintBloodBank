# GmintBloodBank – AGENTS.md

## Two projects

| Project | Path | State |
|---|---|---|
| **GmintBloodBank (v2)** | `GmintBloodBank/` | Full backend: Domain, Application (custom CQRS), Infrastructure (EF Core + multi-tenant + licensing), API |
| BloodBankManagementSystem (v1) | `BloodBankManagementSystem/` | **Does not exist on disk.** Was never scaffolded. The frontend at `GmintBloodBank/Frontend/` is the default Vite template — no business code yet. |

**Always work in v2.** If a task targets "v1" or mentions frontend, note that `Frontend/` is scaffold-only.

## Architecture (v2, verified against code)

- Clean Architecture: `API → Application → Domain ← Infrastructure`
- **Custom CQRS** — `ICommand<TResponse>`/`IQuery<TResponse>` + reflection-based dispatchers (`ICommandDispatcher`, `IQueryDispatcher`). **No MediatR.**
- **Manual mappers** — `IMapper<TEntity, TDto>` with `ToDto`/`ToEntity`. **No AutoMapper.**
- Multi-tenant: `ITenantContext`, `TenantResolutionMiddleware`, `TenantSaveChangesInterceptor`, `ITenantEntity`
- Licensing + feature flags: `ILicenseService`, `IFeatureFlagService`, `RequireFeatureFlagAttribute`
- `.NET 10` (`net10.0`, `<Nullable>enable</Nullable>`), PostgreSQL 17 via EF Core + Npgsql, JWT Bearer + refresh tokens, Serilog
- ~32 business entities under `Domain/Entities/` organized by module (Security, Donors, Donations, BloodUnits, Inventory, Notifications, Tenancy, Licensing)

## SQL schema

`BloodBankManagementSystem_v2.sql` (1403 lines) — full schema for all 8 modules, seed data, operational views. Auto-loaded by `docker-compose.yml` into PostgreSQL.

## Commands

Run all dotnet commands from the `GmintBloodBank/Backend/` directory.

```powershell
docker compose up -d                          # Start PostgreSQL (port 5432)
dotnet build                                  # Uses .slnx; 0 err, ~78 CS8618 warnings (expected for EF)
dotnet test UnitTests\UnitTests.csproj        # xUnit + Moq + FluentAssertions, no Docker needed
dotnet test IntegrationTests\IntegrationTests.csproj  # xUnit + Testcontainers.PostgreSql — Docker required
dotnet run --project API\API.csproj           # HTTP :5210 / HTTPS :7082

# Frontend (scaffold-only, no business code):
cd GmintBloodBank/Frontend
npm run dev                                   # Vite dev server
npm run build                                 # tsc -b && vite build
npm run lint                                  # eslint .
```

## Testing quirks

- Integration tests use **Testcontainers.PostgreSql** — **Docker Desktop must be running.**
- Both test projects have only placeholder `UnitTest1.cs`. No real tests exist yet.
- Coverage via `coverlet.collector`.

## Gotchas

- Solution uses `.slnx` (new XML format) — `dotnet build` resolves correctly from the `Backend/` dir without explicit solution file.
- No `.editorconfig`, no formatter config (Prettier/dotnet format), no CI/CD, no pre-commit hooks, no codegen, no `opencode.json`.
- 5 empty behavior decorator files exist in `Application/Common/Behaviors/` (ValidationDecorator, LoggingDecorator, TransactionDecorator, TenantGuardDecorator, LicenseGuardDecorator) — planned extension points, no implementation.
- `appsettings.json` has local Postgres/JWT values; `appsettings.Development.json` is minimal (overrides logging only).
- `BloodBankManagementSystem_v2.sql` at repo root — NOT inside the v2 project tree.
- **No `BloodBankManagementSystem/` (v1) directory exists.** The `BloodBankManagementSystem.ps1` script is a *scaffold generator* for new projects, not the current project's build tool.
- Frontend connects to nothing yet — API URL not configured in frontend.
