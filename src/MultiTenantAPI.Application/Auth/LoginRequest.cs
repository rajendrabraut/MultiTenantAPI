namespace MultiTenantAPI.Application.Auth;

public sealed record LoginRequest(string Username, string Password, string CompanyCode);
