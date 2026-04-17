using Domain.Models.DailyMythdle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations;

public class DailyMythdleConfiguration : IEntityTypeConfiguration<DailyMythdle>
{
    public void Configure(EntityTypeBuilder<DailyMythdle> builder)
    {
        builder.ToTable("daily_mythdles");

        builder.HasKey(dm => dm.Id);

        builder
            .Property(dm => dm.PuzzleDate)
            .IsRequired();

        builder
            .HasIndex(dm => dm.PuzzleDate)
            .HasDatabaseName("ix_daily_mythdles_puzzle_date");

        builder
            .Property(dm => dm.MythdleTargetName)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(dm => dm.TargetNames)
            .IsRequired()
            .HasColumnType("jsonb")
            .HasColumnName("target_names");

        builder
            .HasOne(dm => dm.Target)
            .WithMany()
            .HasForeignKey(dm => dm.MythdleTargetName)
            .HasPrincipalKey(t => t.Name)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
