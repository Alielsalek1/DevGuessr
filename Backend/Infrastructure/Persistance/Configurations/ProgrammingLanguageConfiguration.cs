using Domain.Models.ProgrammingLanguage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations;

public class ProgrammingLanguageConfiguration : IEntityTypeConfiguration<ProgrammingLanguage>
{
    public void Configure(EntityTypeBuilder<ProgrammingLanguage> builder)
    {
        builder.HasKey(pl => pl.Id);

        builder
            .HasIndex(pl => pl.Name)
            .IsUnique();

        builder
            .Property(pl => pl.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(pl => pl.YearFirstAppeared)
            .IsRequired();

        builder
            .Property(pl => pl.TypingDiscipline)
            .IsRequired();

        builder
            .Property(pl => pl.TypeStrength)
            .IsRequired();

        builder
            .Property(pl => pl.ExecutionModel)
            .IsRequired();

        builder
            .Property(pl => pl.MemoryManagement)
            .IsRequired();

        builder
            .Property(pl => pl.Tags)
            .HasColumnType("jsonb")
            .IsRequired();
    }
}
