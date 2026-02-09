namespace MultiTenantAPI.Application.Tenants;

public sealed class TenantSessionOptions
{
    public const string SectionName = "TenantSession";

    public TenantSessionMode Mode { get; set; } = TenantSessionMode.Cookie;
    public TimeSpan SessionLifetime { get; set; } = TimeSpan.FromHours(8);
    public string CookieName { get; set; } = "tenant-session";
}
