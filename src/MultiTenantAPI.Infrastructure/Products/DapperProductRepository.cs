using Dapper;
using MultiTenantAPI.Application.Abstractions;
using MultiTenantAPI.Application.Products;

namespace MultiTenantAPI.Infrastructure.Products;

public sealed class DapperProductRepository : IProductRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DapperProductRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT Id, Name, Price FROM Products ORDER BY Name";
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<ProductDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }
}
