using MultiTenantAPI.Application.Tenants;

namespace MultiTenantAPI.Infrastructure.Tenants;

public sealed class TenantContextAccessor : ITenantContextAccessor
{
    public TenantContext? Current { get; set; }
}
