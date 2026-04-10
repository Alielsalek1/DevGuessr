using Domain.Enums;

namespace Domain.Models.User;

public class GuestUserCreationParams
{
    public Roles Role { get; set; } = Roles.Guest;
}