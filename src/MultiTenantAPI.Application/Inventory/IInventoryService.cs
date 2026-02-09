namespace MultiTenantAPI.Application.Inventory;

public interface IInventoryService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<SupplierDto>> GetSuppliersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<InventoryItemDto>> GetItemsAsync(CancellationToken cancellationToken);
    Task<InventoryItemDto> CreateItemAsync(CreateInventoryItemRequest request, CancellationToken cancellationToken);
    Task<StockMovementDto> AdjustStockAsync(AdjustStockRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<LowStockItemDto>> GetLowStockAsync(CancellationToken cancellationToken);
}
