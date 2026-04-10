using Domain.Models.DailyDevGuessr;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations;

public class DailyDevGuessrConfiguration : IEntityTypeConfiguration<DailyDevGuessr>
{
    public void Configure(EntityTypeBuilder<DailyDevGuessr> builder)
    {
        builder.ToTable("daily_techdles");

        builder.HasKey(dt => dt.Id);
        builder
            .HasIndex(dt => dt.PuzzleDate)
            .IsUnique();

        builder
            .Property(dt => dt.PuzzleDate)
            .IsRequired();

        builder
            .HasOne(dt => dt.TargetLanguage)
            .WithMany()
            .HasForeignKey(dt => dt.ProgrammingLanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
