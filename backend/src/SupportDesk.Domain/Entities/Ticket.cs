using SupportDesk.Domain.Enums;
using SupportDesk.Domain.Exceptions;

namespace SupportDesk.Domain.Entities;

public sealed class Ticket
{
    private Ticket()
    {
    }

    private Ticket(Guid id, string title, string description, TicketPriority priority, Guid createdByUserId, DateTimeOffset now)
    {
        if (id == Guid.Empty)
        {
            throw new DomainValidationException(nameof(id), "El identificador del ticket es obligatorio.");
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new DomainValidationException(nameof(createdByUserId), "El creador es obligatorio.");
        }

        Id = id;
        Title = Normalize(title, nameof(title), 5, 120);
        Description = Normalize(description, nameof(description), 10, 2000);
        Priority = ValidatePriority(priority);
        Status = TicketStatus.Open;
        CreatedByUserId = createdByUserId;
        CreatedAt = now.ToUniversalTime();
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public User? CreatedByUser { get; private set; }
    public ICollection<Comment> Comments { get; private set; } = new List<Comment>();

    public static Ticket Create(Guid id, string title, string description, TicketPriority priority, Guid createdByUserId, DateTimeOffset now)
        => new(id, title, description, priority, createdByUserId, now);

    public void Update(string title, string description, TicketPriority priority, DateTimeOffset now)
    {
        if (Status == TicketStatus.Closed)
        {
            throw new BusinessConflictException("No se puede editar un ticket cerrado.");
        }

        Title = Normalize(title, nameof(title), 5, 120);
        Description = Normalize(description, nameof(description), 10, 2000);
        Priority = ValidatePriority(priority);
        UpdatedAt = now.ToUniversalTime();
    }

    public void ChangeStatus(TicketStatus nextStatus, DateTimeOffset now)
    {
        var expected = Status switch
        {
            TicketStatus.Open => TicketStatus.InProgress,
            TicketStatus.InProgress => TicketStatus.Resolved,
            TicketStatus.Resolved => TicketStatus.Closed,
            _ => (TicketStatus?)null
        };

        if (expected != nextStatus)
        {
            throw new BusinessConflictException($"La transición de {Status} a {nextStatus} no está permitida.");
        }

        Status = nextStatus;
        UpdatedAt = now.ToUniversalTime();
    }

    public Comment AddComment(Guid commentId, string text, Guid createdByUserId, DateTimeOffset now)
    {
        if (Status == TicketStatus.Closed)
        {
            throw new BusinessConflictException("No se pueden agregar comentarios a un ticket cerrado.");
        }

        var comment = Comment.Create(commentId, Id, text, createdByUserId, now);
        Comments.Add(comment);
        UpdatedAt = now.ToUniversalTime();
        return comment;
    }

    private static string Normalize(string value, string field, int minimumLength, int maximumLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length < minimumLength || normalized.Length > maximumLength)
        {
            throw new DomainValidationException(field, $"{field} debe contener entre {minimumLength} y {maximumLength} caracteres.");
        }

        return normalized;
    }

    private static TicketPriority ValidatePriority(TicketPriority priority)
    {
        if (!Enum.IsDefined(typeof(TicketPriority), priority))
        {
            throw new DomainValidationException(nameof(priority), "La prioridad no es válida.");
        }

        return priority;
    }
}
