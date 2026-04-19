using Application.Constants.ApiErrors;
using Application.Services.Implementations;
using Application.Repositories.Interfaces;
using Domain.Enums;
using Moq;
using Xunit;

using LangdleModel = Domain.Models.Langdle.Langdle;
using LangdleCreationParams = Domain.Models.Langdle.LangdleCreationParams;

namespace Tests.IntergrationTests.DailyEngine.GameCreation;

public class LangdleBatchConflictUnitTests
{
    private static LangdleModel CreateLanguage(string name)
    {
        return new LangdleModel(new LangdleCreationParams
        {
            Name = name,
            YearFirstAppeared = 2000,
            TypeChecking = TypeChecking.STATIC,
            Memory = Memory.MANUAL,
            ScopeSyntax = ScopeSyntax.BRACES,
            Semicolons = Semicolons.REQUIRED,
            Tags = ["systems"]
        });
    }

    [Fact]
    public async Task CreateGamesAsync_WhenRangeInsertFails_ReturnsPuzzleAlreadyExistsError()
    {
        var gameRepo = new Mock<ILangdleGameRepository>();
        var langRepo = new Mock<ILangdleRepository>();

        langRepo
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateLanguage("LangA"), CreateLanguage("LangB")]);

        gameRepo
            .Setup(x => x.GetLatestPuzzleDateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateOnly?)null);

        gameRepo
            .Setup(x => x.TryAddRangeAsync(It.IsAny<IReadOnlyCollection<Domain.Models.DailyLangdle.DailyLangdle>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new LangdlePlayerService(gameRepo.Object, langRepo.Object);

        var result = await service.CreateGamesAsync(CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(AdminLangdleErrorCodes.PuzzleAlreadyExists, result.Error.errorCode);

        gameRepo.Verify(
            x => x.TryAddRangeAsync(It.IsAny<IReadOnlyCollection<Domain.Models.DailyLangdle.DailyLangdle>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
