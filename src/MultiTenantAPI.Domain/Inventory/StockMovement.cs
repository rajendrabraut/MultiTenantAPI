namespace MultiTenantAPI.Domain.Inventory;

public sealed class StockMovement
{
    public Guid Id { get; set; }
    public Guid InventoryItemId { get; set; }
    public int QuantityDelta { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime OccurredAtUtc { get; set; }
}
