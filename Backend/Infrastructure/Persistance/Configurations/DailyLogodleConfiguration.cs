using Domain.Models.DailyLogodle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations;

public class DailyLogodleConfiguration : IEntityTypeConfiguration<DailyLogodle>
{
	public void Configure(EntityTypeBuilder<DailyLogodle> builder)
	{
		builder.HasKey(dl => dl.Id);

		builder
			.HasIndex(dl => dl.PuzzleDate)
			.IsUnique();

		builder
			.Property(dl => dl.PuzzleDate)
			.IsRequired();

		builder
			.HasOne(dl => dl.Target)
			.WithMany()
			.HasForeignKey(dl => dl.LogodleTargetId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}