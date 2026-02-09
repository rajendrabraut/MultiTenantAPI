using System.Data;
using Microsoft.Data.SqlClient;
using MultiTenantAPI.Application.Abstractions;
using MultiTenantAPI.Application.Tenants;

namespace MultiTenantAPI.Infrastructure.Persistence;

public sealed class TenantDbConnectionFactory : IDbConnectionFactory
{
    private readonly ITenantProvider _tenantProvider;

    public TenantDbConnectionFactory(ITenantProvider tenantProvider)
    {
        _tenantProvider = tenantProvider;
    }

    public IDbConnection CreateConnection()
    {
        var tenant = _tenantProvider.GetCurrentTenant();
        var connection = new SqlConnection(tenant.ConnectionString);
        connection.Open();
        return connection;
    }
}
