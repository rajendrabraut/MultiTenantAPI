namespace MultiTenantAPI.API.Configuration;

public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimiting";

    public int PermitLimit { get; set; } = 10000;
    public int WindowSeconds { get; set; } = 60;
}
