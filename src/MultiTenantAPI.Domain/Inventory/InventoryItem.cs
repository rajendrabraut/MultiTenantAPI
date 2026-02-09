namespace MultiTenantAPI.Domain.Inventory;

public sealed class InventoryItem
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Guid SupplierId { get; set; }
    public int ReorderPoint { get; set; }
    public decimal UnitCost { get; set; }
    public bool IsActive { get; set; }
}
