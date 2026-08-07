using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Infrastructure.Persistence.Configurations;

internal sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");
        builder.HasKey(ticket => ticket.Id).HasName("PK_Tickets");
        builder.Property(ticket => ticket.Title).HasMaxLength(120).IsRequired();
        builder.Property(ticket => ticket.Description).HasMaxLength(2000).IsRequired();
        builder.Property(ticket => ticket.Priority).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(ticket => ticket.Status).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.HasOne(ticket => ticket.CreatedByUser).WithMany().HasForeignKey(ticket => ticket.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(ticket => ticket.CreatedByUserId).HasDatabaseName("IX_Tickets_CreatedByUserId");
        builder.HasIndex(ticket => new { ticket.Status, ticket.Priority, ticket.CreatedAt }).HasDatabaseName("IX_Tickets_Status_Priority_CreatedAt");
    }
}
