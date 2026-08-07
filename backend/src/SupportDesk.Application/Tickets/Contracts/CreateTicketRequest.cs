using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Tickets.Contracts;

public sealed record CreateTicketRequest(string Title, string Description, TicketPriority Priority);
