using Microsoft.EntityFrameworkCore;
using SupportDesk.Application.Abstractions;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Infrastructure.Persistence;

public sealed class UserRepository : IUserRepository
{
    private readonly SupportDeskDbContext _dbContext;

    public UserRepository(SupportDeskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetOrCreateByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var existing = await _dbContext.Users.SingleOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var displayName = normalizedEmail.Split('@')[0];
        var user = new User(Guid.NewGuid(), normalizedEmail, displayName);
        await _dbContext.Users.AddAsync(user, cancellationToken);
        return user;
    }
}
