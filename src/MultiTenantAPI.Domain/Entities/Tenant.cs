namespace MultiTenantAPI.Domain.Entities;

public sealed class Tenant
{
    public Guid Id { get; set; }
    public string CompanyCode { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
