using System.Data;

namespace MultiTenantAPI.Application.Abstractions;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
