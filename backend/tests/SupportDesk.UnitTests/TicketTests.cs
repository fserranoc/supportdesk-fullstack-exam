using System;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;
using SupportDesk.Domain.Exceptions;
using Xunit;

namespace SupportDesk.UnitTests;

public sealed class TicketTests
{
    private static readonly Guid UserId = Guid.Parse("ba3285a5-2bd4-42ae-a1ac-560d23e3f328");
    private static readonly DateTimeOffset Now = new(2026, 8, 3, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidValues_NormalizesAndSetsServerFields()
    {
        var ticket = Ticket.Create(Guid.NewGuid(), "  Error de acceso  ", "  No puedo ingresar al portal  ", TicketPriority.High, UserId, Now);

        Assert.Equal("Error de acceso", ticket.Title);
        Assert.Equal("No puedo ingresar al portal", ticket.Description);
        Assert.Equal(TicketStatus.Open, ticket.Status);
        Assert.Equal(Now, ticket.CreatedAt);
        Assert.Equal(Now, ticket.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abcd")]
    public void Create_WithInvalidTitle_ThrowsValidation(string title)
    {
        Assert.Throws<DomainValidationException>(() => Ticket.Create(Guid.NewGuid(), title, "Descripción suficientemente larga", TicketPriority.Medium, UserId, Now));
    }

    [Theory]
    [InlineData("")]
    [InlineData("demasiado")]
    public void Create_WithInvalidDescription_ThrowsValidation(string description)
    {
        Assert.Throws<DomainValidationException>(() => Ticket.Create(Guid.NewGuid(), "Título válido", description, TicketPriority.Medium, UserId, Now));
    }

    [Fact]
    public void ChangeStatus_FollowsRequiredSequence()
    {
        var ticket = CreateTicket();

        ticket.ChangeStatus(TicketStatus.InProgress, Now.AddMinutes(1));
        ticket.ChangeStatus(TicketStatus.Resolved, Now.AddMinutes(2));
        ticket.ChangeStatus(TicketStatus.Closed, Now.AddMinutes(3));

        Assert.Equal(TicketStatus.Closed, ticket.Status);
    }

    [Fact]
    public void ChangeStatus_WhenSkippingState_ThrowsConflict()
    {
        var ticket = CreateTicket();

        Assert.Throws<BusinessConflictException>(() => ticket.ChangeStatus(TicketStatus.Resolved, Now));
    }

    [Theory]
    [InlineData(TicketStatus.Open, TicketStatus.InProgress)]
    [InlineData(TicketStatus.InProgress, TicketStatus.Resolved)]
    [InlineData(TicketStatus.Resolved, TicketStatus.Closed)]
    public void ChangeStatus_WithNextState_Succeeds(TicketStatus initial, TicketStatus next)
    {
        var ticket = CreateTicket();
        if (initial >= TicketStatus.InProgress)
        {
            ticket.ChangeStatus(TicketStatus.InProgress, Now.AddMinutes(1));
        }
        if (initial >= TicketStatus.Resolved)
        {
            ticket.ChangeStatus(TicketStatus.Resolved, Now.AddMinutes(2));
        }

        ticket.ChangeStatus(next, Now.AddMinutes(3));

        Assert.Equal(next, ticket.Status);
    }

    [Fact]
    public void AddComment_WhenTicketIsClosed_ThrowsConflict()
    {
        var ticket = CreateTicket();
        ticket.ChangeStatus(TicketStatus.InProgress, Now);
        ticket.ChangeStatus(TicketStatus.Resolved, Now);
        ticket.ChangeStatus(TicketStatus.Closed, Now);

        Assert.Throws<BusinessConflictException>(() => ticket.AddComment(Guid.NewGuid(), "Comentario válido", UserId, Now));
    }

    [Theory]
    [InlineData("")]
    [InlineData("x")]
    public void AddComment_WithInvalidText_ThrowsValidation(string text)
    {
        var ticket = CreateTicket();

        Assert.Throws<DomainValidationException>(() => ticket.AddComment(Guid.NewGuid(), text, UserId, Now));
    }

    [Fact]
    public void Update_WhenTicketIsClosed_ThrowsConflict()
    {
        var ticket = CreateTicket();
        ticket.ChangeStatus(TicketStatus.InProgress, Now);
        ticket.ChangeStatus(TicketStatus.Resolved, Now);
        ticket.ChangeStatus(TicketStatus.Closed, Now);

        Assert.Throws<BusinessConflictException>(() => ticket.Update("Título actualizado", "Descripción actualizada del incidente", TicketPriority.Low, Now));
    }

    private static Ticket CreateTicket() => Ticket.Create(Guid.NewGuid(), "Error de acceso", "No puedo ingresar al portal", TicketPriority.High, UserId, Now);
}
