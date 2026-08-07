using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Infrastructure.Persistence.Configurations;

internal sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");
        builder.HasKey(comment => comment.Id).HasName("PK_Comments");
        builder.Property(comment => comment.Text).HasMaxLength(1000).IsRequired();
        builder.HasOne(comment => comment.Ticket).WithMany(ticket => ticket.Comments).HasForeignKey(comment => comment.TicketId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(comment => comment.CreatedByUser).WithMany().HasForeignKey(comment => comment.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(comment => comment.TicketId).HasDatabaseName("IX_Comments_TicketId");
        builder.HasIndex(comment => comment.CreatedByUserId).HasDatabaseName("IX_Comments_CreatedByUserId");
    }
}
