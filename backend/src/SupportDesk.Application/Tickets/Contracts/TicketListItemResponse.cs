using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Tickets.Contracts;

public sealed record TicketListItemResponse(
    Guid Id,
    string Title,
    TicketPriority Priority,
    TicketStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string CreatedBy)
{
    public static TicketListItemResponse From(Ticket ticket) => new(
        ticket.Id,
        ticket.Title,
        ticket.Priority,
        ticket.Status,
        ticket.CreatedAt,
        ticket.UpdatedAt,
        ticket.CreatedByUser?.Email ?? string.Empty);
}
