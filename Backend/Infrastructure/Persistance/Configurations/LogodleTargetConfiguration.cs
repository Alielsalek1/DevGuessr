using Domain.Models.LogodleTarget;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations;

public class LogodleTargetConfiguration : IEntityTypeConfiguration<LogodleTarget>
{
    public void Configure(EntityTypeBuilder<LogodleTarget> builder)
    {
        builder.HasKey(lt => lt.Id);

        builder
            .HasIndex(lt => lt.Name)
            .IsUnique();

        builder
            .Property(lt => lt.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(lt => lt.ImagePath)
            .HasMaxLength(500)
            .IsRequired();

        builder
            .Property(lt => lt.BlurredImageUrls)
            .HasColumnType("jsonb")
            .IsRequired();
    }
}
