using Application.DTOs.Archive;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;

public interface IGameArchiveService
{
    Task<Result<SuccessApiResponse<GetPastGamesResponseDto>>> GetPastGamesAsync(GetPastGamesRequestDto request, CancellationToken cancellationToken);
}
