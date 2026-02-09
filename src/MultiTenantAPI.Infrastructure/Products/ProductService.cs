using MultiTenantAPI.Application.Products;

namespace MultiTenantAPI.Infrastructure.Products;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken cancellationToken)
    {
        return _repository.GetProductsAsync(cancellationToken);
    }
}
