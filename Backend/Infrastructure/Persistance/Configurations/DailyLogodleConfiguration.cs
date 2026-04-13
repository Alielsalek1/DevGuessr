using Domain.Models.DailyLogodle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations;

public class DailyLogodleConfiguration : IEntityTypeConfiguration<DailyLogodle>
{
	public void Configure(EntityTypeBuilder<DailyLogodle> builder)
	{
		builder.ToTable("daily_logodles");

		builder.HasKey(dl => dl.Id);

		builder
			.Property(dl => dl.PuzzleDate)
			.IsRequired();

		builder
			.HasIndex(dl => dl.PuzzleDate)
			.HasDatabaseName("ix_daily_logodles_puzzle_date");

		builder
			.Property(dl => dl.LogodleTargetName)
			.HasMaxLength(100)
			.IsRequired();

		builder
			.HasOne(dl => dl.Target)
			.WithMany()
			.HasForeignKey(dl => dl.LogodleTargetName)
			.HasPrincipalKey(t => t.Name)
			.OnDelete(DeleteBehavior.Restrict);
	}
}