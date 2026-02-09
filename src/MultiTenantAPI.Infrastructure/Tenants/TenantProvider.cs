using Microsoft.Extensions.Logging;
using MultiTenantAPI.Application.Tenants;

namespace MultiTenantAPI.Infrastructure.Tenants;

public sealed class TenantProvider : ITenantProvider
{
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly ILogger<TenantProvider> _logger;

    public TenantProvider(ITenantContextAccessor tenantContextAccessor, ILogger<TenantProvider> logger)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _logger = logger;
    }

    public TenantContext GetCurrentTenant()
    {
        if (!TryGetCurrentTenant(out var tenant) || tenant is null)
        {
            _logger.LogWarning("Tenant context is missing for the current request.");
            throw new InvalidOperationException("Tenant context is missing.");
        }

        return tenant;
    }

    public bool TryGetCurrentTenant(out TenantContext? tenant)
    {
        tenant = _tenantContextAccessor.Current;
        return tenant is not null;
    }
}
