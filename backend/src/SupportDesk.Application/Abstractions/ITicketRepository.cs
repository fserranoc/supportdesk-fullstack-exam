using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Abstractions;

public interface ITicketRepository
{
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken);
    Task AddCommentAsync(Comment comment, CancellationToken cancellationToken);
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<(IReadOnlyList<Ticket> Items, int Total)> SearchAsync(TicketSearchCriteria criteria, CancellationToken cancellationToken);
    Task<IReadOnlyList<Comment>> GetCommentsAsync(Guid ticketId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public sealed record TicketSearchCriteria(
    TicketStatus? Status,
    TicketPriority? Priority,
    string? Query,
    int Page,
    int PageSize,
    string SortBy,
    bool Descending);
