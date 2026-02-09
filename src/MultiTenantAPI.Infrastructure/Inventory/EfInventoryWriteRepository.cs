using Microsoft.EntityFrameworkCore;
using MultiTenantAPI.Application.Common;
using MultiTenantAPI.Application.Inventory;
using MultiTenantAPI.Domain.Inventory;
using MultiTenantAPI.Infrastructure.Persistence;

namespace MultiTenantAPI.Infrastructure.Inventory;

public sealed class EfInventoryWriteRepository : IInventoryWriteRepository
{
    private readonly TenantDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public EfInventoryWriteRepository(TenantDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<InventoryItemDto> CreateItemAsync(CreateInventoryItemRequest request, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.InventoryItems.AnyAsync(i => i.Sku == request.Sku, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("SKU already exists.");
        }

        var item = new InventoryItem
        {
            Id = Guid.NewGuid(),
            Sku = request.Sku,
            Name = request.Name,
            CategoryId = request.CategoryId,
            SupplierId = request.SupplierId,
            ReorderPoint = request.ReorderPoint,
            UnitCost = request.UnitCost,
            IsActive = true
        };

        _dbContext.InventoryItems.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new InventoryItemDto(
            item.Id,
            item.Sku,
            item.Name,
            item.CategoryId,
            item.SupplierId,
            item.ReorderPoint,
            item.UnitCost,
            item.IsActive);
    }

    public async Task<StockMovementDto> AddStockMovementAsync(AdjustStockRequest request, CancellationToken cancellationToken)
    {
        var item = await _dbContext.InventoryItems.FirstOrDefaultAsync(i => i.Id == request.InventoryItemId, cancellationToken);
        if (item is null || !item.IsActive)
        {
            throw new InvalidOperationException("Inventory item not found.");
        }

        var movement = new StockMovement
        {
            Id = Guid.NewGuid(),
            InventoryItemId = item.Id,
            QuantityDelta = request.QuantityDelta,
            Reason = request.Reason,
            OccurredAtUtc = _dateTimeProvider.UtcNow
        };

        _dbContext.StockMovements.Add(movement);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new StockMovementDto(
            movement.Id,
            movement.InventoryItemId,
            movement.QuantityDelta,
            movement.Reason,
            movement.OccurredAtUtc);
    }
}
