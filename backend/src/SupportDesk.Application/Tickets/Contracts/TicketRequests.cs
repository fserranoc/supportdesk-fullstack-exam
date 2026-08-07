using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Tickets.Contracts;

public sealed record UpdateTicketRequest(string Title, string Description, TicketPriority Priority);
public sealed record ChangeTicketStatusRequest(TicketStatus Status);
public sealed record CreateCommentRequest(string Text);

public sealed record TicketQueryRequest(
    TicketStatus? Status,
    TicketPriority? Priority,
    string? Q,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "createdAt",
    string SortDirection = "desc");
