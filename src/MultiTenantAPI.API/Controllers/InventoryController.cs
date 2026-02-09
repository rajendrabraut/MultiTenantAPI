using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantAPI.Application.Inventory;

namespace MultiTenantAPI.API.Controllers;

[ApiController]
[Authorize]
[Route("api/inventory")]
public sealed class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _inventoryService.GetCategoriesAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("suppliers")]
    public async Task<ActionResult<IReadOnlyList<SupplierDto>>> GetSuppliers(CancellationToken cancellationToken)
    {
        var suppliers = await _inventoryService.GetSuppliersAsync(cancellationToken);
        return Ok(suppliers);
    }

    [HttpGet("items")]
    public async Task<ActionResult<IReadOnlyList<InventoryItemDto>>> GetItems(CancellationToken cancellationToken)
    {
        var items = await _inventoryService.GetItemsAsync(cancellationToken);
        return Ok(items);
    }

    [HttpPost("items")]
    public async Task<ActionResult<InventoryItemDto>> CreateItem([FromBody] CreateInventoryItemRequest request, CancellationToken cancellationToken)
    {
        var item = await _inventoryService.CreateItemAsync(request, cancellationToken);
        return Ok(item);
    }

    [HttpPost("stock/adjust")]
    public async Task<ActionResult<StockMovementDto>> AdjustStock([FromBody] AdjustStockRequest request, CancellationToken cancellationToken)
    {
        var movement = await _inventoryService.AdjustStockAsync(request, cancellationToken);
        return Ok(movement);
    }

    [HttpGet("stock/low")]
    public async Task<ActionResult<IReadOnlyList<LowStockItemDto>>> GetLowStock(CancellationToken cancellationToken)
    {
        var items = await _inventoryService.GetLowStockAsync(cancellationToken);
        return Ok(items);
    }
}
