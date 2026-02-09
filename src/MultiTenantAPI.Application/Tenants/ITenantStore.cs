using MultiTenantAPI.Domain.Entities;

namespace MultiTenantAPI.Application.Tenants;

public interface ITenantStore
{
    Task<Tenant?> GetByCompanyCodeAsync(string companyCode, CancellationToken cancellationToken);
}
