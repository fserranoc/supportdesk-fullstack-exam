using SupportDesk.Domain.Entities;

namespace SupportDesk.Application.Tickets.Contracts;

public sealed record CommentResponse(Guid Id, Guid TicketId, string Text, DateTimeOffset CreatedAt, string CreatedBy)
{
    public static CommentResponse From(Comment comment, string? createdBy = null) => new(
        comment.Id,
        comment.TicketId,
        comment.Text,
        comment.CreatedAt,
        createdBy ?? comment.CreatedByUser?.Email ?? string.Empty);
}
