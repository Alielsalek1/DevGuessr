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
            .Property(pl => pl.TypeChecking)
            .IsRequired();

        builder
            .Property(pl => pl.Memory)
            .IsRequired();

        builder
            .Property(pl => pl.ScopeSyntax)
            .IsRequired();

        builder
            .Property(pl => pl.Semicolons)
            .IsRequired();

        builder
            .Property(pl => pl.Tags)
            .HasColumnType("jsonb")
            .IsRequired();
    }
}
