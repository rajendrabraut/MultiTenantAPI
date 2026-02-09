using Microsoft.Extensions.Logging;
using MultiTenantAPI.Application.Tenants;

namespace MultiTenantAPI.Application.Auth;

public sealed class AuthService : IAuthService
{
    private readonly ITenantStore _tenantStore;
    private readonly ITenantSessionService _tenantSessionService;
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ITenantStore tenantStore,
        ITenantSessionService tenantSessionService,
        ITenantContextAccessor tenantContextAccessor,
        IUserService userService,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _tenantStore = tenantStore;
        _tenantSessionService = tenantSessionService;
        _tenantContextAccessor = tenantContextAccessor;
        _userService = userService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantStore.GetByCompanyCodeAsync(request.CompanyCode, cancellationToken);
        if (tenant is null || !tenant.IsActive)
        {
            _logger.LogWarning("Login failed. Unknown or inactive company code {CompanyCode}.", request.CompanyCode);
            throw new UnauthorizedAccessException("Invalid company code.");
        }

        _tenantContextAccessor.Current = new TenantContext(tenant.CompanyCode, tenant.ConnectionString, null);
        var session = await _tenantSessionService.CreateSessionAsync(tenant.CompanyCode, tenant.ConnectionString, cancellationToken);

        var user = await _userService.ValidateCredentialsAsync(request.Username, request.Password, cancellationToken);
        if (user is null)
        {
            await _tenantSessionService.ClearSessionAsync(session.TenantSessionId, cancellationToken);
            _logger.LogWarning("Login failed for {Username} in {CompanyCode}.", request.Username, request.CompanyCode);
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var token = _tokenService.CreateAccessToken(user.Id.ToString(), user.Username, tenant.CompanyCode, session.TenantSessionId);
        return new LoginResponse(token, "Bearer", _tokenService.GetAccessTokenLifetimeSeconds());
    }
}
