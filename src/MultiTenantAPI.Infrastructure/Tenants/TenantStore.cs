using Microsoft.EntityFrameworkCore;
using MultiTenantAPI.Application.Tenants;
using MultiTenantAPI.Domain.Entities;
using MultiTenantAPI.Infrastructure.Persistence;

namespace MultiTenantAPI.Infrastructure.Tenants;

public sealed class TenantStore : ITenantStore
{
    private readonly MasterDbContext _dbContext;

    public TenantStore(MasterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Tenant?> GetByCompanyCodeAsync(string companyCode, CancellationToken cancellationToken)
    {
        return _dbContext.Tenants.AsNoTracking()
            .FirstOrDefaultAsync(t => t.CompanyCode == companyCode, cancellationToken);
    }
}
