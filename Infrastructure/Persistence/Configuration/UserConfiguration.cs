using Domain.Entities;
using Domain.ValueObj;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneOf.Types;

namespace Infrastructure.Persistence.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(128).IsRequired();
        builder.OwnsOne(x => x.Email, b =>
        {
            b.Property(x => x.Email).HasMaxLength(256).IsRequired();
            b.HasIndex(x => x.Email).IsUnique();
        });
        builder.OwnsOne(x => x.Password, b =>
        {
            b.Property(x => x.Salt).HasMaxLength(32).IsFixedLength().IsRequired();
            b.Property(x => x.Hashed).HasMaxLength(256).IsFixedLength().IsRequired();
        });
        builder.OwnsOne(x => x.Secret, b =>
        {
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.Bearer).HasMaxLength(4096).IsFixedLength().IsRequired();
            b.HasIndex(x => x.Bearer).IsUnique();
        });
        builder.Navigation(x => x.Secret).IsRequired(false);

    }
}