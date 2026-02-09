namespace MultiTenantAPI.Application.Inventory;

public interface IInventoryWriteRepository
{
    Task<InventoryItemDto> CreateItemAsync(CreateInventoryItemRequest request, CancellationToken cancellationToken);
    Task<StockMovementDto> AddStockMovementAsync(AdjustStockRequest request, CancellationToken cancellationToken);
}
