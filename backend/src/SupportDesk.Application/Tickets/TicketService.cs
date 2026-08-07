using SupportDesk.Application.Abstractions;
using SupportDesk.Application.Exceptions;
using SupportDesk.Application.Tickets.Contracts;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Exceptions;

namespace SupportDesk.Application.Tickets;

public sealed class TicketService
{
    private readonly ITicketRepository _tickets;
    private readonly IUserRepository _users;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;

    public TicketService(ITicketRepository tickets, IUserRepository users, ICurrentUserService currentUser, IClock clock)
    {
        _tickets = tickets;
        _users = users;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<TicketDetailResponse> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        var user = await _users.GetOrCreateByEmailAsync(_currentUser.Email, cancellationToken);
        var ticket = Ticket.Create(Guid.NewGuid(), request.Title, request.Description, request.Priority, user.Id, _clock.UtcNow);

        await _tickets.AddAsync(ticket, cancellationToken);
        await _tickets.SaveChangesAsync(cancellationToken);

        return TicketDetailResponse.From(ticket, user.Email);
    }

    public async Task<TicketDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var ticket = await _tickets.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("El ticket solicitado no existe.");

        return TicketDetailResponse.From(ticket);
    }

    public async Task<PagedResponse<TicketListItemResponse>> SearchAsync(TicketQueryRequest request, CancellationToken cancellationToken)
    {
        if (request.Page < 1 || request.PageSize is < 1 or > 100)
        {
            throw new DomainValidationException(nameof(request.Page), "page debe ser al menos 1 y pageSize debe estar entre 1 y 100.");
        }

        var query = string.IsNullOrWhiteSpace(request.Q) ? null : request.Q.Trim();
        if (query?.Length > 200)
        {
            throw new DomainValidationException(nameof(request.Q), "q admite hasta 200 caracteres.");
        }

        var sortBy = request.SortBy.Trim().ToLowerInvariant();
        if (sortBy is not ("createdat" or "updatedat" or "title" or "priority" or "status"))
        {
            throw new DomainValidationException(nameof(request.SortBy), "sortBy no contiene una columna permitida.");
        }

        var direction = request.SortDirection.Trim().ToLowerInvariant();
        if (direction is not ("asc" or "desc"))
        {
            throw new DomainValidationException(nameof(request.SortDirection), "sortDirection debe ser asc o desc.");
        }

        var criteria = new TicketSearchCriteria(request.Status, request.Priority, query, request.Page, request.PageSize, sortBy, direction == "desc");
        var result = await _tickets.SearchAsync(criteria, cancellationToken);
        var totalPages = (int)Math.Ceiling(result.Total / (double)request.PageSize);
        return new PagedResponse<TicketListItemResponse>(result.Items.Select(TicketListItemResponse.From).ToList(), request.Page, request.PageSize, result.Total, totalPages);
    }

    public async Task<TicketDetailResponse> UpdateAsync(Guid id, UpdateTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await GetEntityAsync(id, cancellationToken);
        ticket.Update(request.Title, request.Description, request.Priority, _clock.UtcNow);
        await _tickets.SaveChangesAsync(cancellationToken);
        return TicketDetailResponse.From(ticket);
    }

    public async Task<TicketDetailResponse> ChangeStatusAsync(Guid id, ChangeTicketStatusRequest request, CancellationToken cancellationToken)
    {
        var ticket = await GetEntityAsync(id, cancellationToken);
        ticket.ChangeStatus(request.Status, _clock.UtcNow);
        await _tickets.SaveChangesAsync(cancellationToken);
        return TicketDetailResponse.From(ticket);
    }

    public async Task<CommentResponse> AddCommentAsync(Guid ticketId, CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var ticket = await GetEntityAsync(ticketId, cancellationToken);
        var user = await _users.GetOrCreateByEmailAsync(_currentUser.Email, cancellationToken);
        var comment = ticket.AddComment(Guid.NewGuid(), request.Text, user.Id, _clock.UtcNow);
        await _tickets.AddCommentAsync(comment, cancellationToken);
        await _tickets.SaveChangesAsync(cancellationToken);
        return CommentResponse.From(comment, user.Email);
    }

    public async Task<IReadOnlyList<CommentResponse>> GetCommentsAsync(Guid ticketId, CancellationToken cancellationToken)
    {
        _ = await GetEntityAsync(ticketId, cancellationToken);
        var comments = await _tickets.GetCommentsAsync(ticketId, cancellationToken);
        return comments.Select(comment => CommentResponse.From(comment)).ToList();
    }

    private async Task<Ticket> GetEntityAsync(Guid id, CancellationToken cancellationToken)
        => await _tickets.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("El ticket solicitado no existe.");
}
