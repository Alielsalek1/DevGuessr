using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Application.Repositories.Interfaces;
using Domain.Models.User;

namespace Infrastructure.Repositories.Implementations;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    public async Task AddUserAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetUserByGoogleIdAsync(string googleId, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.GoogleId == googleId, cancellationToken);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> ConfirmEmailAsync(string email, CancellationToken cancellationToken)
    {
        var rowsAffected = await _dbContext.Users
            .Where(u => u.Email == email && u.IsEmailVerified == false)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.IsEmailVerified, true),
                cancellationToken);

        return rowsAffected > 0;
    }

    public async Task<bool> IsEmailInUseAsync(string email, CancellationToken cancellationToken)
    {
        bool isEmailTaken = await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == email, cancellationToken);
        return isEmailTaken;
    }

    public async Task<bool> IsPhoneNumberInUseAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        bool isPhoneNumberTaken = await _dbContext.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
        return isPhoneNumberTaken;
    }

    public async Task<bool> IsUsernameInUseAsync(string username, CancellationToken cancellationToken)
    {
        bool isUsernameTaken = await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Username == username, cancellationToken);
        return isUsernameTaken;
    }

    public async Task<bool> IsEmailConfirmedAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        return user?.IsEmailVerified ?? false;
    }
    public async Task<bool> UpdatePasswordByEmailAsync(string email, string newPasswordHash, CancellationToken cancellationToken)
    {
        var rowsAffected = await _dbContext.Users
            .Where(u => u.Email == email)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.PasswordHash, newPasswordHash),
                cancellationToken);
        return rowsAffected > 0;
    }


}