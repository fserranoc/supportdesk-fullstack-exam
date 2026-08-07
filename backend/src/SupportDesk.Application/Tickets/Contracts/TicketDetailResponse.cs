using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Tickets.Contracts;

public sealed record TicketDetailResponse(
    Guid Id,
    string Title,
    string Description,
    TicketPriority Priority,
    TicketStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string CreatedBy)
{
    public static TicketDetailResponse From(Ticket ticket, string? createdBy = null) => new(
        ticket.Id,
        ticket.Title,
        ticket.Description,
        ticket.Priority,
        ticket.Status,
        ticket.CreatedAt,
        ticket.UpdatedAt,
        createdBy ?? ticket.CreatedByUser?.Email ?? string.Empty);
}
