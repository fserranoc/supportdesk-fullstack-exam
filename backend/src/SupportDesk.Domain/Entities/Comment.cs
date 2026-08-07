using SupportDesk.Domain.Exceptions;

namespace SupportDesk.Domain.Entities;

public sealed class Comment
{
    private Comment()
    {
    }

    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public Ticket? Ticket { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public User? CreatedByUser { get; private set; }

    public static Comment Create(Guid id, Guid ticketId, string text, Guid createdByUserId, DateTimeOffset now)
    {
        if (id == Guid.Empty || ticketId == Guid.Empty || createdByUserId == Guid.Empty)
        {
            throw new DomainValidationException(nameof(id), "Los identificadores del comentario son obligatorios.");
        }

        var normalized = text?.Trim() ?? string.Empty;
        if (normalized.Length < 2 || normalized.Length > 1000)
        {
            throw new DomainValidationException(nameof(text), "text debe contener entre 2 y 1000 caracteres.");
        }

        return new Comment
        {
            Id = id,
            TicketId = ticketId,
            Text = normalized,
            CreatedByUserId = createdByUserId,
            CreatedAt = now.ToUniversalTime()
        };
    }
}
