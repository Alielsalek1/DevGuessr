namespace Infrastructure.Repositories.Implementations;

using Domain.Models.UserRefreshTokens;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Application.Repositories.Interfaces;

public class UserRefreshTokensRepository(AppDbContext dbContext) : IUserRefreshTokensRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddUserRefreshTokenAsync(UserRefreshToken userRefreshToken, CancellationToken cancellationToken)
    {
        await _dbContext.UserRefreshTokens.AddAsync(userRefreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserRefreshToken?> GetUserRefreshTokenAsync(Guid userId, string refreshTokenHash, CancellationToken cancellationToken)
    {
        return await _dbContext.UserRefreshTokens.AsNoTracking()
            .FirstOrDefaultAsync(urt => urt.UserId == userId && urt.RefreshTokenHash == refreshTokenHash, cancellationToken);
    }

    public async Task DeleteUserRefreshTokenAsync(Guid userId, string refreshTokenHash, CancellationToken cancellationToken)
    {
        await _dbContext.UserRefreshTokens
            .Where(urt => urt.UserId == userId && urt.RefreshTokenHash == refreshTokenHash)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task MarkTokenAsUsedAsync(Guid userId, string refreshTokenHash, CancellationToken cancellationToken)
    {
        var token = await _dbContext.UserRefreshTokens
            .FirstOrDefaultAsync(urt => urt.UserId == userId && urt.RefreshTokenHash == refreshTokenHash, cancellationToken);
        
            token!.MarkAsUsed();
            await _dbContext.SaveChangesAsync(cancellationToken);
    }
}