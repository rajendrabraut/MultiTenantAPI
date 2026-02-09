namespace MultiTenantAPI.Application.Auth;

public sealed record LoginResponse(string AccessToken, string TokenType, int ExpiresInSeconds);
