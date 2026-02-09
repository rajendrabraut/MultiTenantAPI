using Microsoft.AspNetCore.Authorization;
using MultiTenantAPI.Application.Tenants;

namespace MultiTenantAPI.API.Middleware;

public sealed class TenantMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(
        HttpContext context,
        ITenantSessionService tenantSessionService,
        ITenantContextAccessor tenantContextAccessor,
        ILogger<TenantMiddleware> logger)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
        {
            await _next(context);
            return;
        }

        var tenantSessionId = tenantSessionService.GetSessionIdFromRequest();
        var session = await tenantSessionService.TryGetSessionAsync(tenantSessionId, context.RequestAborted);
        if (session is null)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Tenant session is missing or invalid.");
                return;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized.");
            return;
        }

        tenantContextAccessor.Current = new TenantContext(session.CompanyCode, session.ConnectionString, session.TenantSessionId);
        logger.LogDebug("Tenant context set for {CompanyCode}.", session.CompanyCode);

        await _next(context);
    }
}
