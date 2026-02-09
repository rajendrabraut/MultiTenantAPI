namespace MultiTenantAPI.Application.Products;

public sealed record ProductDto(Guid Id, string Name, decimal Price);
