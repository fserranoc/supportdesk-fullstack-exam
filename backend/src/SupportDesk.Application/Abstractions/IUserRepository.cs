using SupportDesk.Domain.Entities;

namespace SupportDesk.Application.Abstractions;

public interface IUserRepository
{
    Task<User> GetOrCreateByEmailAsync(string email, CancellationToken cancellationToken);
}
