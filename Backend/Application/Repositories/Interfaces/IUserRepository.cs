using Domain.Models.User;

namespace Application.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> IsUsernameInUseAsync(string username, CancellationToken cancellationToken);
    Task<bool> IsEmailInUseAsync(string email, CancellationToken cancellationToken);
    Task<bool> IsPhoneNumberInUseAsync(string phoneNumber, CancellationToken cancellationToken);
    Task<bool> IsEmailConfirmedAsync(string email, CancellationToken cancellationToken);
    Task<bool> ConfirmEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> UpdatePasswordByEmailAsync(string email, string newPassword, CancellationToken cancellationToken);

    Task AddUserAsync(User user, CancellationToken cancellationToken);

    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetUserByGoogleIdAsync(string googleId, CancellationToken cancellationToken);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken);
}
