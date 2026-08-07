using Microsoft.EntityFrameworkCore;
using SupportDesk.Application.Abstractions;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Infrastructure.Persistence;

public sealed class TicketRepository : ITicketRepository
{
    private readonly SupportDeskDbContext _dbContext;

    public TicketRepository(SupportDeskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Ticket ticket, CancellationToken cancellationToken)
        => _dbContext.Tickets.AddAsync(ticket, cancellationToken).AsTask();

    public Task AddCommentAsync(Comment comment, CancellationToken cancellationToken)
        => _dbContext.Comments.AddAsync(comment, cancellationToken).AsTask();

    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _dbContext.Tickets.Include(ticket => ticket.CreatedByUser).SingleOrDefaultAsync(ticket => ticket.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Ticket> Items, int Total)> SearchAsync(TicketSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var query = _dbContext.Tickets.AsNoTracking().Include(ticket => ticket.CreatedByUser).AsQueryable();

        if (criteria.Status.HasValue)
        {
            query = query.Where(ticket => ticket.Status == criteria.Status.Value);
        }

        if (criteria.Priority.HasValue)
        {
            query = query.Where(ticket => ticket.Priority == criteria.Priority.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Query))
        {
            var pattern = $"%{criteria.Query}%";
            query = query.Where(ticket => EF.Functions.Like(ticket.Title, pattern) || EF.Functions.Like(ticket.Description, pattern));
        }

        var total = await query.CountAsync(cancellationToken);
        query = ApplyOrder(query, criteria.SortBy, criteria.Descending);
        var items = await query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<Comment>> GetCommentsAsync(Guid ticketId, CancellationToken cancellationToken)
        => await _dbContext.Comments
            .AsNoTracking()
            .Include(comment => comment.CreatedByUser)
            .Where(comment => comment.TicketId == ticketId)
            .OrderBy(comment => comment.CreatedAt)
            .ThenBy(comment => comment.Id)
            .ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Ticket> ApplyOrder(IQueryable<Ticket> query, string sortBy, bool descending)
        => (sortBy, descending) switch
        {
            ("updatedat", false) => query.OrderBy(ticket => ticket.UpdatedAt).ThenBy(ticket => ticket.Id),
            ("updatedat", true) => query.OrderByDescending(ticket => ticket.UpdatedAt).ThenBy(ticket => ticket.Id),
            ("title", false) => query.OrderBy(ticket => ticket.Title).ThenBy(ticket => ticket.Id),
            ("title", true) => query.OrderByDescending(ticket => ticket.Title).ThenBy(ticket => ticket.Id),
            ("priority", false) => query.OrderBy(ticket => ticket.Priority).ThenBy(ticket => ticket.Id),
            ("priority", true) => query.OrderByDescending(ticket => ticket.Priority).ThenBy(ticket => ticket.Id),
            ("status", false) => query.OrderBy(ticket => ticket.Status).ThenBy(ticket => ticket.Id),
            ("status", true) => query.OrderByDescending(ticket => ticket.Status).ThenBy(ticket => ticket.Id),
            ("createdat", false) => query.OrderBy(ticket => ticket.CreatedAt).ThenBy(ticket => ticket.Id),
            _ => query.OrderByDescending(ticket => ticket.CreatedAt).ThenBy(ticket => ticket.Id)
        };
}
