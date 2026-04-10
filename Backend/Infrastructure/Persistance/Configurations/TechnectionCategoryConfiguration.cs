using Domain.Models.TechnectionCategory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations;

public class TechnectionCategoryConfiguration : IEntityTypeConfiguration<TechnectionCategory>
{
    public void Configure(EntityTypeBuilder<TechnectionCategory> builder)
    {
        builder.HasKey(tc => tc.Id);

        builder
            .HasIndex(tc => tc.GroupName)
            .IsUnique();

        builder
            .Property(tc => tc.GroupName)
            .HasMaxLength(200)
            .IsRequired();

        builder
            .Property(tc => tc.DifficultyLevel)
            .IsRequired();

        builder
            .Property(tc => tc.Words)
            .HasColumnType("jsonb")
            .IsRequired();

        builder
            .Property(tc => tc.SuccessMessage)
            .HasMaxLength(500)
            .IsRequired();
    }
}
