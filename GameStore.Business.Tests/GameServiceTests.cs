using AutoFixture;
using AutoFixture.AutoMoq;
using Business.DTO;
using Business.Interfaces;
using Business.Services;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using FluentAssertions;
using Moq;

namespace GameStore.Business.Tests;

public class GameServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly IGameService _gameService;
    private readonly CancellationToken _ct;

    public GameServiceTests()
    {
        _ct = new CancellationTokenSource().Token;

        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _mockUnitOfWork = _fixture.Freeze<Mock<IUnitOfWork>>();
        _mockGameRepository = new Mock<IGameRepository>();
        _mockUnitOfWork.SetupGet(u => u.GameRepository).Returns(_mockGameRepository.Object);

        _gameService = _fixture.Create<GameService>();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllGames()
    {
        // Arrange
        var fakeGames = _fixture.CreateMany<Game>();
        _mockGameRepository.Setup(u => u.GetAllAsync(_ct))
            .ReturnsAsync(fakeGames);

        // Act
        var result = await _gameService.GetAllAsync(_ct);

        // Assert
        Assert.NotNull(result);
        _mockGameRepository.Verify(x => x.GetAllAsync(_ct), Times.Once);
    }

    [Fact]
    public async Task AddAsync_AddGameToTheList()
    {
        // Arrange
        var fakeGame = _fixture.Create<CreateGameDto>();

        // Act
        await _gameService.AddAsync(fakeGame, _ct);

        // Assert
        _mockGameRepository.Verify(u => u.AddAsync(It.IsAny<Game>(), _ct), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeleteSelectedEntity()
    {
        // Arrange
        var fakeGame = _fixture.Create<Game>();

        _mockGameRepository.Setup(u => u.GetByIdAsync(It.IsAny<int>(), _ct))
            .ReturnsAsync(fakeGame);

        // Act
        await _gameService.DeleteAsync(fakeGame.Alias, _ct);

        // Assert
        _mockGameRepository.Verify(u => u.DeleteAsync(fakeGame, _ct), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdateSelectedEntity()
    {
        // Arrange
        var fakeGame = _fixture.Create<Game>();
        var fakeGameDto = _fixture.Create<GameDto>();
        _mockGameRepository.Setup(u => u.GetByIdAsync(It.IsAny<int>(), _ct))
            .ReturnsAsync(fakeGame);

        // Act
        await _gameService.UpdateAsync(fakeGameDto, _ct);

        // Assert
        _mockGameRepository.Verify(u => u.Update(It.IsAny<Game>()), Times.Once);
    }

    [Fact]
    public async Task GetGamesByPlatformId_ReturnGamesBySelectedGenre()
    {
        // Arrange
        var fakeGames = _fixture.Create<Game>();

        _mockGameRepository.Setup(u => u.GetByIdAsync(It.IsAny<int>(), _ct))
            .ReturnsAsync(fakeGames);

        // Act
        var result = await _gameService.GetGamesByPlatformId(fakeGames.Id, _ct);

        // Assert
        result.Should().NotBeNull();
        _mockGameRepository.Verify(x => x.GetGamesByPlatformId(fakeGames.Id, _ct), Times.Once);
    }
}