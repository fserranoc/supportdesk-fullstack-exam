using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Application.Exceptions;
using SupportDesk.Domain.Exceptions;

namespace SupportDesk.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await WriteProblemAsync(context, exception);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        var (status, title, detail) = exception switch
        {
            DomainValidationException => (StatusCodes.Status400BadRequest, "Solicitud inválida", exception.Message),
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Solicitud inválida", exception.Message),
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado", exception.Message),
            BusinessConflictException => (StatusCodes.Status409Conflict, "Conflicto de negocio", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Error interno", "Ocurrió un error inesperado.")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled error. TraceId: {TraceId}", context.TraceIdentifier);
        }

        var problem = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{status}",
            Title = title,
            Status = status,
            Detail = detail,
            Instance = context.Request.Path
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;
        if (exception is DomainValidationException validationException)
        {
            problem.Extensions["errors"] = new Dictionary<string, string[]>
            {
                [validationException.Field] = new[] { validationException.Message }
            };
        }

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await JsonSerializer.SerializeAsync(context.Response.Body, problem, cancellationToken: context.RequestAborted);
    }
}
