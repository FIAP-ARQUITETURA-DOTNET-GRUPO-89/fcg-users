using FcgUsers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FcgUsers.Infra.EntityTypeConfigurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
               .IsRequired()
               .HasColumnType("timestamp with time zone")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.BirthDate)
               .IsRequired()
               .HasColumnType("date");

        // Configuração do ValueObject Email
        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(e => e.Address)
                 .HasColumnName("Email")
                 .HasMaxLength(254)
                 .IsRequired();

            email.HasIndex(e => e.Address).IsUnique();
        });

        builder.OwnsOne(x => x.Password, password =>
        {
            password.Property(p => p.Hash)
                 .HasColumnName("Password")
                 .HasMaxLength(60)
                 .IsRequired();
        });

        builder.Property(x => x.Role)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(20);

        builder.Property(x => x.IsInactive)
               .IsRequired()
               .HasDefaultValue(false);
    }
}
