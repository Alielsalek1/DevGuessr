namespace Infrastructure.Persistance.Configurations;

using Domain.Models.User;
using Domain.Models.UserDevice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserDevicesConfiguration : IEntityTypeConfiguration<UserDevice>
{
    public void Configure(EntityTypeBuilder<UserDevice> builder)
    {
        builder
            .HasKey(ud => new { ud.UserId, ud.DeviceId });
        // mark userId as foregin key to Users table
        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(ud => ud.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}