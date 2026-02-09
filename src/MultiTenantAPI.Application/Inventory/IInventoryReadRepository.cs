namespace MultiTenantAPI.Application.Inventory;

public interface IInventoryReadRepository
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<SupplierDto>> GetSuppliersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<InventoryItemDto>> GetItemsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<LowStockItemDto>> GetLowStockAsync(CancellationToken cancellationToken);
}
