using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id).HasName("PK_Users");
        builder.Property(user => user.Email).HasMaxLength(254).IsRequired();
        builder.Property(user => user.DisplayName).HasMaxLength(120).IsRequired();
        builder.HasIndex(user => user.Email).IsUnique().HasDatabaseName("UX_Users_Email");
    }
}
