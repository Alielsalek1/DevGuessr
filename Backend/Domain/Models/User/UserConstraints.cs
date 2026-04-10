namespace Domain.Constraints.User;
public class UserConstraints
{
    public const int UsernameMinLength = 5;
    public const int UsernameMaxLength = 25;
    public const int PasswordHashLength = 60;
    public const int PasswordMinLength = 6;
    public const int PasswordMaxLength = 50;
    public const int EmailMaxLength = 150;
    public const int AddressMaxLength = 200;
    public const int PhoneNumberMaxLength = 15;
}