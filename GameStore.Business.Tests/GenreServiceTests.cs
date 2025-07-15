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

public class GenreServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGenreRepository> _mockGenreRepository;
    private readonly CancellationToken _ct;
    private readonly IGenreService _genreService;

    public GenreServiceTests()
    {
        _ct = new CancellationTokenSource().Token;

        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _mockUnitOfWork = _fixture.Freeze<Mock<IUnitOfWork>>();
        _mockGenreRepository = new Mock<IGenreRepository>();
        _mockUnitOfWork.SetupGet(x => x.GenreRepository).Returns(_mockGenreRepository.Object);

        _genreService = _fixture.Create<GenreService>();
    }

    [Fact]
    public async Task CreateGenreAsync_CreateNewEntity()
    {
        // Arrange
        var fakeGenre = _fixture.Create<GenreDto>();

        // Act
        await _genreService.CreateGenreAsync(fakeGenre, _ct);

        // Assert
        _mockGenreRepository.Verify(x => x.AddAsync(It.IsAny<Genre>(), _ct), Times.Once);
    }

    [Fact]
    public async Task DeleteGenreAsync_DeleteEntity()
    {
        // Arrange
        var fakeGenre = _fixture.Create<Genre>();
        _mockGenreRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), _ct))
            .ReturnsAsync(fakeGenre);

        // Act
        await _genreService.DeleteGenreAsync(fakeGenre.Name, _ct);

        // Assert
        _mockGenreRepository.Verify(x => x.DeleteAsync(fakeGenre, _ct), Times.Once);
    }

    [Fact]
    public async Task GetAllGenresAsync_ReturnsAllGenres()
    {
        // Arrange
        var fakeGenres = _fixture.CreateMany<Genre>();
        _mockGenreRepository.Setup(x => x.GetAllAsync(_ct))
            .ReturnsAsync(fakeGenres);

        // Act
        var result = await _genreService.GetAllGenresAsync(_ct);

        // Assert
        Assert.NotNull(result);
        _mockGenreRepository.Verify(x => x.GetAllAsync(_ct), Times.Once);
    }

    [Fact]
    public async Task GetGenreByIdAsync_ReturnSingleValue()
    {
        // Arrange
        var fakeGenre = _fixture.Create<Genre>();

        _mockGenreRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), _ct))
            .ReturnsAsync(fakeGenre);

        // Act
        var genre = await _genreService.GetGenreByIdAsync(fakeGenre.Id, _ct);

        // Assert
        _mockGenreRepository.Verify(x => x.GetByIdAsync(fakeGenre.Id, _ct), Times.Once);
        genre.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateGenreAsync_UpdateEntity()
    {
        // Arrange
        var fakeGenre = _fixture.Create<Genre>();
        var fakeGenreDto = _fixture.Create<GenreDto>();
        _mockGenreRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), _ct))
            .ReturnsAsync(fakeGenre);

        // Act
        await _genreService.UpdateGenreAsync(fakeGenreDto, _ct);

        // Assert
        _mockGenreRepository.Verify(x => x.Update(It.IsAny<Genre>()), Times.Once);
    }
}
