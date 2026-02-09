namespace MultiTenantAPI.Application.Tenants;

public sealed record TenantContext(
    string CompanyCode,
    string ConnectionString,
    string? TenantSessionId);
