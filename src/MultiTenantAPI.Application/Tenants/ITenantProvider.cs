namespace MultiTenantAPI.Application.Tenants;

public interface ITenantProvider
{
    TenantContext GetCurrentTenant();
    bool TryGetCurrentTenant(out TenantContext? tenant);
}
