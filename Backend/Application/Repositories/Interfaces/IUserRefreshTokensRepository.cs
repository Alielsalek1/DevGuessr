namespace Application.Repositories.Interfaces;

using Domain.Models.UserRefreshTokens;

public interface IUserRefreshTokensRepository
{
    Task AddUserRefreshTokenAsync(UserRefreshToken userRefreshToken, CancellationToken cancellationToken);
    Task<UserRefreshToken?> GetUserRefreshTokenAsync(Guid userId, string refreshTokenHash, CancellationToken cancellationToken);
    Task DeleteUserRefreshTokenAsync(Guid userId, string refreshTokenHash, CancellationToken cancellationToken);
    Task MarkTokenAsUsedAsync(Guid userId, string refreshTokenHash, CancellationToken cancellationToken);
}