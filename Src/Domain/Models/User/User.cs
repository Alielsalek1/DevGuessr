using Domain.Constraints.User;
using Domain.Enums;

namespace Domain.Models.User;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string? GoogleId { get; private set; }
    public string? Username { get; private set; }
    public string? PasswordHash { get; set; }
    public string? Email { get; private set; }
    public bool IsEmailVerified { get; private set; } = false;
    public Roles Role { get; private set; }
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    // Parameterless constructor for EF Core
    private User() { }

    public bool IsGuest() => Role == Roles.Guest;

    public User(UserCreationParams userCreationParams)
    {
        UserGuard.ValidateUsername(userCreationParams.Username);
        UserGuard.ValidatePasswordHash(userCreationParams.PasswordHash);
        UserGuard.ValidateEmail(userCreationParams.Email);
        UserGuard.ValidatePhoneNumber(userCreationParams.PhoneNumber);
        UserGuard.ValidateAddress(userCreationParams.Address);

        Username = userCreationParams.Username;
        PasswordHash = userCreationParams.PasswordHash;
        Email = userCreationParams.Email;
        Role = userCreationParams.Role;
        Address = userCreationParams.Address ?? string.Empty;
        PhoneNumber = userCreationParams.PhoneNumber ?? string.Empty;
    }

    public User(ExternalUserCreationParams externalUserCreationParams)
    {
        UserGuard.ValidateEmail(externalUserCreationParams.Email);

        GoogleId = externalUserCreationParams.GoogleId;
        Email = externalUserCreationParams.Email;
        Role = externalUserCreationParams.Role;
        IsEmailVerified = true;
    }

    public User(GuestUserCreationParams guestUserCreationParams)
    {
        Role = guestUserCreationParams.Role;
    }

    public void UpdateAddress(string? address)
    {
        UserGuard.ValidateAddress(address);
        Address = address ?? string.Empty;
    }

    public void UpdatePhoneNumber(string? phoneNumber)
    {
        UserGuard.ValidatePhoneNumber(phoneNumber);
        PhoneNumber = phoneNumber ?? string.Empty;
    }

    public void MarkEmailAsVerified()
    {
        IsEmailVerified = true;
    }

    public void SetGuestId(Guid guestId)
    {
        Id = guestId;
    }

    public void UpdateUserToBeEligibleForExternalLogin(string email, string googleId)
    {
        UserGuard.ValidateEmail(email);

        Email = email;
        GoogleId = googleId;
        Role = Roles.User;
        IsEmailVerified = true;
    }
}
