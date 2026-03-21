namespace Infrastructure.Persistance.Configurations;

using Domain.Models.User;
using Domain.Models.UserRefreshTokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserRefreshTokensConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder
            .HasKey(urt => new { urt.UserId, urt.RefreshTokenHash });
        builder
            .Property(urt => urt.RefreshTokenHash)
            .HasMaxLength(64)
            .IsRequired();
        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(urt => urt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}