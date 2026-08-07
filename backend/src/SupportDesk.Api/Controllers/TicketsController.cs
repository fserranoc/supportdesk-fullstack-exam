using Microsoft.AspNetCore.Mvc;
using SupportDesk.Application.Tickets;
using SupportDesk.Application.Tickets.Contracts;

namespace SupportDesk.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public sealed class TicketsController : ControllerBase
{
    private readonly TicketService _ticketService;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(TicketService ticketService, ILogger<TicketsController> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }

    /// <summary>Crea un ticket con estado inicial Open.</summary>
    /// <response code="201">El ticket fue creado.</response>
    /// <response code="400">Los datos o el encabezado X-User son inválidos.</response>
    /// <response code="500">Ocurrió un error inesperado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TicketDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TicketDetailResponse>> Create(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        var created = await _ticketService.CreateAsync(request, cancellationToken);
        _logger.LogInformation("Ticket {TicketId} created", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Lista tickets con filtros, búsqueda, orden y paginación de servidor.</summary>
    /// <response code="200">Devuelve la página solicitada.</response>
    /// <response code="400">Los filtros o parámetros de paginación son inválidos.</response>
    /// <response code="500">Ocurrió un error inesperado.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<TicketListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResponse<TicketListItemResponse>>> Search([FromQuery] TicketQueryRequest request, CancellationToken cancellationToken)
        => Ok(await _ticketService.SearchAsync(request, cancellationToken));

    /// <summary>Obtiene el detalle de un ticket.</summary>
    /// <response code="200">Devuelve el ticket.</response>
    /// <response code="404">El ticket no existe.</response>
    /// <response code="500">Ocurrió un error inesperado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TicketDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TicketDetailResponse>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _ticketService.GetByIdAsync(id, cancellationToken));

    /// <summary>Actualiza título, descripción y prioridad de un ticket.</summary>
    /// <response code="200">Devuelve el ticket actualizado.</response>
    /// <response code="400">Los datos son inválidos.</response>
    /// <response code="404">El ticket no existe.</response>
    /// <response code="409">El ticket está cerrado y no admite edición.</response>
    /// <response code="500">Ocurrió un error inesperado.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TicketDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TicketDetailResponse>> Update(Guid id, UpdateTicketRequest request, CancellationToken cancellationToken)
        => Ok(await _ticketService.UpdateAsync(id, request, cancellationToken));

    /// <summary>Avanza un ticket al siguiente estado permitido.</summary>
    /// <response code="200">Devuelve el ticket con su nuevo estado.</response>
    /// <response code="400">El estado enviado es inválido.</response>
    /// <response code="404">El ticket no existe.</response>
    /// <response code="409">La transición solicitada no está permitida.</response>
    /// <response code="500">Ocurrió un error inesperado.</response>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(TicketDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TicketDetailResponse>> ChangeStatus(Guid id, ChangeTicketStatusRequest request, CancellationToken cancellationToken)
    {
        var updated = await _ticketService.ChangeStatusAsync(id, request, cancellationToken);
        _logger.LogInformation("Ticket {TicketId} status changed to {Status}", updated.Id, updated.Status);
        return Ok(updated);
    }

    /// <summary>Agrega un comentario a un ticket abierto.</summary>
    /// <response code="201">El comentario fue creado.</response>
    /// <response code="400">El comentario o el encabezado X-User son inválidos.</response>
    /// <response code="404">El ticket no existe.</response>
    /// <response code="409">El ticket está cerrado.</response>
    /// <response code="500">Ocurrió un error inesperado.</response>
    [HttpPost("{id}/comments")]
    [ProducesResponseType(typeof(CommentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CommentResponse>> AddComment(Guid id, CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var comment = await _ticketService.AddCommentAsync(id, request, cancellationToken);
        return CreatedAtAction(nameof(GetComments), new { id }, comment);
    }

    /// <summary>Lista los comentarios de un ticket en orden cronológico.</summary>
    /// <response code="200">Devuelve los comentarios.</response>
    /// <response code="404">El ticket no existe.</response>
    /// <response code="500">Ocurrió un error inesperado.</response>
    [HttpGet("{id}/comments")]
    [ProducesResponseType(typeof(IReadOnlyList<CommentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<CommentResponse>>> GetComments(Guid id, CancellationToken cancellationToken)
        => Ok(await _ticketService.GetCommentsAsync(id, cancellationToken));
}
