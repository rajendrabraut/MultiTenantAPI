using MultiTenantAPI.Domain.Entities;

namespace MultiTenantAPI.Application.Auth;

public interface IUserService
{
    Task<User?> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken);
}
