using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MultiTenantAPI.Application.Tenants;

namespace MultiTenantAPI.Infrastructure.Tenants;

public sealed class CookieTenantSessionService : ITenantSessionService
{
    private readonly IDataProtector _protector;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TenantSessionOptions _options;

    public CookieTenantSessionService(
        IDataProtectionProvider dataProtectionProvider,
        IHttpContextAccessor httpContextAccessor,
        IOptions<TenantSessionOptions> options)
    {
        _protector = dataProtectionProvider.CreateProtector("tenant-session-cookie");
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public Task<TenantSession> CreateSessionAsync(string companyCode, string connectionString, CancellationToken cancellationToken)
    {
        var session = new TenantSession(companyCode, connectionString, null);
        var payload = JsonSerializer.Serialize(session);
        var protectedPayload = _protector.Protect(payload);

        var context = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is not available.");

        context.Response.Cookies.Append(_options.CookieName, protectedPayload, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.Add(_options.SessionLifetime)
        });

        return Task.FromResult(session);
    }

    public Task<TenantSession?> TryGetSessionAsync(string? tenantSessionId, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            return Task.FromResult<TenantSession?>(null);
        }

        if (!context.Request.Cookies.TryGetValue(_options.CookieName, out var payload))
        {
            return Task.FromResult<TenantSession?>(null);
        }

        try
        {
            var json = _protector.Unprotect(payload);
            var session = JsonSerializer.Deserialize<TenantSession>(json);
            return Task.FromResult(session);
        }
        catch
        {
            return Task.FromResult<TenantSession?>(null);
        }
    }

    public Task ClearSessionAsync(string? tenantSessionId, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        context?.Response.Cookies.Delete(_options.CookieName);
        return Task.CompletedTask;
    }

    public string? GetSessionIdFromRequest() => null;
}
