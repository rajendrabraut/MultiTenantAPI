# MultiTenantAPI

## Folder Structure
```
MultiTenantAPI.sln
src/
  MultiTenantAPI.API/
    Controllers/
    Middleware/
    Validation/
    Program.cs
    appsettings.json
    MultiTenantAPI.API.csproj
  MultiTenantAPI.Application/
    Abstractions/
    Auth/
    Products/
    Tenants/
    MultiTenantAPI.Application.csproj
  MultiTenantAPI.Domain/
    Entities/
    MultiTenantAPI.Domain.csproj
  MultiTenantAPI.Infrastructure/
    Auth/
    Persistence/
    Products/
    Tenants/
    MultiTenantAPI.Infrastructure.csproj
tests/
  MultiTenantAPI.Tests/
    ApiSmokeTests.cs
    MultiTenantAPI.Tests.csproj
```

## Overview
This solution implements a multi-tenant ASP.NET Core Web API (.NET 8) using Clean Architecture. Tenant resolution occurs **only at login** via a master database lookup by `companyCode`. Every subsequent request uses a server-side tenant session (cookie or Redis) to set the tenant context, and the tenant connection string is never accepted from client headers or request bodies.

## Key Behaviors
- **Login**: `POST /api/auth/login` with `username`, `password`, `companyCode`.
- **Tenant Resolution**: Master database (`MasterDb`) lookup at login.
- **Tenant Context**: Stored server-side using either:
  - **Encrypted cookie** (default), or
  - **Redis** (session id stored in JWT claim).
- **JWT**: Access token includes `company_code` and `tenant_session_id` (when Redis mode is used).
- **Middleware**: `TenantMiddleware` runs after authentication to set `TenantContext` before controllers.
- **EF Core + Dapper**: Both use the per-request tenant connection string.

## Configuration (Cookie vs Redis)
Edit `src/MultiTenantAPI.API/appsettings.json`:

```json
"TenantSession": {
  "Mode": "Cookie",
  "SessionLifetime": "08:00:00",
  "CookieName": "mt-tenant"
}
```

Set `Mode` to `Redis` to use distributed cache backed sessions. Ensure Redis connection string is configured.

### Rate Limiting
```json
"RateLimiting": {
  "PermitLimit": 10000,
  "WindowSeconds": 60
}
```

## Migration Notes
### Master DB (Tenant Store)
```bash
dotnet ef migrations add InitialMaster -p src/MultiTenantAPI.Infrastructure -s src/MultiTenantAPI.API -c MasterDbContext
```

### Tenant DB (Per Tenant)
```bash
dotnet ef migrations add InitialTenant -p src/MultiTenantAPI.Infrastructure -s src/MultiTenantAPI.API -c TenantDbContext
```

Apply migrations for each database:
```bash
dotnet ef database update -p src/MultiTenantAPI.Infrastructure -s src/MultiTenantAPI.API -c MasterDbContext
```

For tenant databases, run the migration once per tenant database connection string.

## Example SQL (Seed Data)
Master DB tenant registration:
```sql
INSERT INTO Tenants (Id, CompanyCode, ConnectionString, IsActive)
VALUES (NEWID(), 'ACME', 'Server=localhost;Database=Tenant_ACME;User Id=sa;Password=Your_password123;TrustServerCertificate=True;', 1);
```

Tenant DB sample user and products:
```sql
INSERT INTO Users (Id, Username, PasswordHash, IsActive)
VALUES (NEWID(), 'admin', '<hashed-password>', 1);

INSERT INTO Products (Id, Name, Price)
VALUES (NEWID(), 'Sample Product', 19.99);
```

## Example Requests
### Login
```
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Password123!",
  "companyCode": "ACME"
}
```

Response:
```json
{
  "accessToken": "<jwt>",
  "tokenType": "Bearer",
  "expiresInSeconds": 3600
}
```

### Get Products
```
GET /api/products
Authorization: Bearer <jwt>
```

Response:
```json
[
  {
    "id": "c3a1b9c5-2d6f-4f1d-9d1f-32c2c9b9f1c2",
    "name": "Sample Product",
    "price": 19.99
  }
]
```

## Security Notes
- Tenant context is derived **only** from server-side session storage (cookie or Redis).
- `companyCode` in JWT is informational; it is not trusted to set the tenant.
- If tenant context is missing or invalid, API returns 401/403.
- SQL Server provider uses standard T-SQL features compatible with SQL Server 2014.
- The API adds correlation IDs and security headers (X-Content-Type-Options, X-Frame-Options, Referrer-Policy) to improve traceability and basic hardening.

## Performance, Bottlenecks, and Threat Protection
- **Rate limiting**: A fixed-window limiter throttles requests per tenant (or per IP when unauthenticated) to protect against floods and abuse.
- **Tenant lookup**: Tenant connection strings are resolved **only at login** from the master store, which avoids per-request lookups.
- **Connection pooling**: ADO.NET pools by connection string. Avoid per-user connection strings; normalize to one connection string per tenant. If you must call an external API for tenant config, cache the resolved connection string at login and store only the server-side session id to avoid per-request fetches.
- **Pool sizing**: Tune `Max Pool Size` per tenant connection string to prevent pool exhaustion and align with SQL Server capacity.
- **Logging & tracing**: Correlation IDs are emitted for every request for auditability and incident response.

## Run the API
```bash
dotnet build MultiTenantAPI.sln
cd src/MultiTenantAPI.API
dotnet run
```

## Tests
```bash
dotnet test MultiTenantAPI.sln
```
