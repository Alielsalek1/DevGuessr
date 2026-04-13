using Domain.Models.Clusterdle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations;

public class ClusterdleConfiguration : IEntityTypeConfiguration<Clusterdle>
{
    public void Configure(EntityTypeBuilder<Clusterdle> builder)
    {
        builder.ToTable("clusterdle_targets");
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
