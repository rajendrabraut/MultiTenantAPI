namespace MultiTenantAPI.Application.Tenants;

public sealed record TenantSession(
    string CompanyCode,
    string ConnectionString,
    string? TenantSessionId);
