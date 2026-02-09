using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiTenantAPI.Application.Auth;
using MultiTenantAPI.Domain.Entities;
using MultiTenantAPI.Infrastructure.Persistence;

namespace MultiTenantAPI.Infrastructure.Auth;

public sealed class UserService : IUserService
{
    private readonly TenantDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(TenantDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success ? user : null;
    }
}
