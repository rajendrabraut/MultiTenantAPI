namespace MultiTenantAPI.Application.Tenants;

public interface ITenantSessionService
{
    Task<TenantSession> CreateSessionAsync(string companyCode, string connectionString, CancellationToken cancellationToken);
    Task<TenantSession?> TryGetSessionAsync(string? tenantSessionId, CancellationToken cancellationToken);
    Task ClearSessionAsync(string? tenantSessionId, CancellationToken cancellationToken);
    string? GetSessionIdFromRequest();
}
