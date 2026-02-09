namespace MultiTenantAPI.Application.Common;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
