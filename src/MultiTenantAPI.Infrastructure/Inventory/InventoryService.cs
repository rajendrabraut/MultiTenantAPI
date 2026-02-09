using MultiTenantAPI.Application.Inventory;

namespace MultiTenantAPI.Infrastructure.Inventory;

public sealed class InventoryService : IInventoryService
{
    private readonly IInventoryReadRepository _readRepository;
    private readonly IInventoryWriteRepository _writeRepository;

    public InventoryService(IInventoryReadRepository readRepository, IInventoryWriteRepository writeRepository)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
    }

    public Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return _readRepository.GetCategoriesAsync(cancellationToken);
    }

    public Task<IReadOnlyList<SupplierDto>> GetSuppliersAsync(CancellationToken cancellationToken)
    {
        return _readRepository.GetSuppliersAsync(cancellationToken);
    }

    public Task<IReadOnlyList<InventoryItemDto>> GetItemsAsync(CancellationToken cancellationToken)
    {
        return _readRepository.GetItemsAsync(cancellationToken);
    }

    public Task<InventoryItemDto> CreateItemAsync(CreateInventoryItemRequest request, CancellationToken cancellationToken)
    {
        return _writeRepository.CreateItemAsync(request, cancellationToken);
    }

    public Task<StockMovementDto> AdjustStockAsync(AdjustStockRequest request, CancellationToken cancellationToken)
    {
        return _writeRepository.AddStockMovementAsync(request, cancellationToken);
    }

    public Task<IReadOnlyList<LowStockItemDto>> GetLowStockAsync(CancellationToken cancellationToken)
    {
        return _readRepository.GetLowStockAsync(cancellationToken);
    }
}
