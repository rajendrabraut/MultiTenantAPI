using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MultiTenantAPI.Application.Auth;
using MultiTenantAPI.Application.Tenants;

namespace MultiTenantAPI.Infrastructure.Tenants;

public sealed class RedisTenantSessionService : ITenantSessionService
{
    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TenantSessionOptions _options;

    public RedisTenantSessionService(
        IDistributedCache cache,
        IHttpContextAccessor httpContextAccessor,
        IOptions<TenantSessionOptions> options)
    {
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public async Task<TenantSession> CreateSessionAsync(string companyCode, string connectionString, CancellationToken cancellationToken)
    {
        var sessionId = Guid.NewGuid().ToString("N");
        var session = new TenantSession(companyCode, connectionString, sessionId);
        var payload = JsonSerializer.Serialize(session);

        await _cache.SetStringAsync(GetCacheKey(sessionId), payload, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _options.SessionLifetime
        }, cancellationToken);

        return session;
    }

    public async Task<TenantSession?> TryGetSessionAsync(string? tenantSessionId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tenantSessionId))
        {
            return null;
        }

        var payload = await _cache.GetStringAsync(GetCacheKey(tenantSessionId), cancellationToken);
        if (string.IsNullOrWhiteSpace(payload))
        {
            return null;
        }

        return JsonSerializer.Deserialize<TenantSession>(payload);
    }

    public Task ClearSessionAsync(string? tenantSessionId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tenantSessionId))
        {
            return Task.CompletedTask;
        }

        return _cache.RemoveAsync(GetCacheKey(tenantSessionId), cancellationToken);
    }

    public string? GetSessionIdFromRequest()
    {
        var context = _httpContextAccessor.HttpContext;
        var claim = context?.User?.FindFirst(TokenClaims.TenantSessionId);
        return claim?.Value;
    }

    private static string GetCacheKey(string sessionId) => $"tenant-session:{sessionId}";
}
