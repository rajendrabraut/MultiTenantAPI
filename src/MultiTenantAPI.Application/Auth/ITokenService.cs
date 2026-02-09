namespace MultiTenantAPI.Application.Auth;

public interface ITokenService
{
    string CreateAccessToken(string userId, string username, string companyCode, string? tenantSessionId);
    int GetAccessTokenLifetimeSeconds();
}
