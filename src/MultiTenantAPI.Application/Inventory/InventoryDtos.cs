namespace MultiTenantAPI.Application.Inventory;

public sealed record CategoryDto(Guid Id, string Name, bool IsActive);
public sealed record SupplierDto(Guid Id, string Name, string Email, bool IsActive);
public sealed record InventoryItemDto(
    Guid Id,
    string Sku,
    string Name,
    Guid CategoryId,
    Guid SupplierId,
    int ReorderPoint,
    decimal UnitCost,
    bool IsActive);

public sealed record StockMovementDto(Guid Id, Guid InventoryItemId, int QuantityDelta, string Reason, DateTime OccurredAtUtc);
public sealed record WarehouseDto(Guid Id, string Name, string Location, bool IsActive);

public sealed record CreateInventoryItemRequest(
    string Sku,
    string Name,
    Guid CategoryId,
    Guid SupplierId,
    int ReorderPoint,
    decimal UnitCost);

public sealed record AdjustStockRequest(Guid InventoryItemId, int QuantityDelta, string Reason);

public sealed record LowStockItemDto(
    Guid InventoryItemId,
    string Sku,
    string Name,
    int CurrentQuantity,
    int ReorderPoint);
