using Domain.Models.MythdleTarget;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations;

public class MythdleTargetConfiguration : IEntityTypeConfiguration<MythdleTarget>
{
    public void Configure(EntityTypeBuilder<MythdleTarget> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Category)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(m => m.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder
            .HasIndex(m => m.Name)
            .IsUnique();
    }
}
