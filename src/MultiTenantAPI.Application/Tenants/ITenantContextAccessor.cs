namespace MultiTenantAPI.Application.Tenants;

public interface ITenantContextAccessor
{
    TenantContext? Current { get; set; }
}
