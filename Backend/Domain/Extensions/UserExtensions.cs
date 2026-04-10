using Domain.Models.User;

namespace Domain.Extensions;

public static class UserExtensions
{
    public static bool IsNotVerified(this User user)
    {
        return !user.IsEmailVerified;
    }

    public static bool IsNotGuest(this User user)
    {
        return !user.IsGuest();
    }

    public static bool IncorrectPassword(this User user, string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash) == false;
    }

    public static bool IsNotEmailVerified(this User user)
    {
        return !user.IsEmailVerified;
    }
}