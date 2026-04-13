using Domain.Models.DailyLangdle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations;

public class DailyLangdleConfiguration : IEntityTypeConfiguration<DailyLangdle>
{
    public void Configure(EntityTypeBuilder<DailyLangdle> builder)
    {
        builder.ToTable("daily_langdles");

        builder.HasKey(dt => dt.Id);

        builder
            .Property(dt => dt.PuzzleDate)
            .IsRequired();

        builder
            .HasIndex(dt => dt.PuzzleDate)
            .HasDatabaseName("ix_daily_langdles_puzzle_date");

        builder
            .Property(dt => dt.LangdleName)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .HasOne(dt => dt.TargetLanguage)
            .WithMany()
            .HasForeignKey(dt => dt.LangdleName)
            .HasPrincipalKey(t => t.Name)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
