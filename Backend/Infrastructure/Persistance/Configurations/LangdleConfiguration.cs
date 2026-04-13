using LangdleModel = Domain.Models.Langdle.Langdle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configurations;

public class LangdleConfiguration : IEntityTypeConfiguration<LangdleModel>
{
    public void Configure(EntityTypeBuilder<LangdleModel> builder)
    {
        builder.ToTable("langdle_targets");
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
