using Dapper;
using MultiTenantAPI.Application.Abstractions;
using MultiTenantAPI.Application.Inventory;

namespace MultiTenantAPI.Infrastructure.Inventory;

public sealed class DapperInventoryReadRepository : IInventoryReadRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DapperInventoryReadRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT Id, Name, IsActive FROM Categories ORDER BY Name";
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<CategoryDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<SupplierDto>> GetSuppliersAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT Id, Name, Email, IsActive FROM Suppliers ORDER BY Name";
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SupplierDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<InventoryItemDto>> GetItemsAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT Id, Sku, Name, CategoryId, SupplierId, ReorderPoint, UnitCost, IsActive FROM InventoryItems ORDER BY Name";
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<InventoryItemDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<LowStockItemDto>> GetLowStockAsync(CancellationToken cancellationToken)
    {
        const string sql = @"
SELECT i.Id AS InventoryItemId,
       i.Sku,
       i.Name,
       COALESCE(SUM(m.QuantityDelta), 0) AS CurrentQuantity,
       i.ReorderPoint
FROM InventoryItems i
LEFT JOIN StockMovements m ON m.InventoryItemId = i.Id
GROUP BY i.Id, i.Sku, i.Name, i.ReorderPoint
HAVING COALESCE(SUM(m.QuantityDelta), 0) <= i.ReorderPoint
ORDER BY i.Name";
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<LowStockItemDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }
}
