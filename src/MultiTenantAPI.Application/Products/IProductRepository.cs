namespace MultiTenantAPI.Application.Products;

public interface IProductRepository
{
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken cancellationToken);
}
