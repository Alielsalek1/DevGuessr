using Domain.Enums;

namespace Domain.Models.User;
public class ExternalUserCreationParams
{
    public required string Email { get; init; }
    public required string GoogleId { get; init; }
    public Roles Role { get; init; }
}