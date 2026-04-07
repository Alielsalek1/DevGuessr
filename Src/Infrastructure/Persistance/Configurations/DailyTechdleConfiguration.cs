using Domain.Models.DailyTechdle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations;

public class DailyTechdleConfiguration : IEntityTypeConfiguration<DailyTechdle>
{
    public void Configure(EntityTypeBuilder<DailyTechdle> builder)
    {
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
