namespace Infrastructure.Persistance.Configurations;

using Domain.Constraints.User;
using Domain.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasKey(u => u.Id);
        builder
            .HasIndex(u => u.GoogleId)
            .IsUnique();
        builder
            .HasIndex(u => u.Username)
            .IsUnique();
        builder
            .HasIndex(u => u.Email)
            .IsUnique();
        builder
            .Property(u => u.Username)
            .HasMaxLength(UserConstraints.UsernameMaxLength);
        builder
            .Property(u => u.PasswordHash)
            .HasMaxLength(UserConstraints.PasswordHashLength);
        builder
            .Property(u => u.Email)
            .HasMaxLength(UserConstraints.EmailMaxLength);
        builder
            .Property(u => u.Role)
            .IsRequired();
        builder
            .Property(u => u.IsEmailVerified)
            .IsRequired();
        builder
            .Property(u => u.Address)
            .HasMaxLength(UserConstraints.AddressMaxLength)
            .HasDefaultValue(string.Empty);
        builder
            .Property(u => u.PhoneNumber)
            .HasMaxLength(UserConstraints.PhoneNumberMaxLength)
            .HasDefaultValue(string.Empty);
    }
}