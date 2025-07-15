using AutoFixture;
using AutoFixture.AutoMoq;
using Business.DTO;
using Business.Interfaces;
using Business.Services;
using Data.SQL.Interfaces;
using FluentAssertions;
using Moq;

namespace GameStore.Business.Tests;

public class PlatformServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IPlatformRepository> _mockPlatformRepository;
    private readonly CancellationToken _ct;
    private readonly IPlatformService _platformService;

    public PlatformServiceTests()
    {
        _ct = new CancellationTokenSource().Token;

        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _mockUnitOfWork = _fixture.Freeze<Mock<IUnitOfWork>>();
        _mockPlatformRepository = new Mock<IPlatformRepository>();
        _mockUnitOfWork.SetupGet(x => x.PlatformRepository).Returns(_mockPlatformRepository.Object);

        _platformService = _fixture.Create<PlatformService>();
    }

    [Fact]
    public async Task CreatePlatformAsync_CreateNewEntity()
    {
        // Arrange
        var fakePlatform = _fixture.Create<PlatformDto>();

        // Act
        await _platformService.CreatePlatformAsync(fakePlatform, _ct);

        // Assert
        _mockPlatformRepository.Verify(x => x.AddAsync(It.IsAny<Data.SQL.Entities.Platform>(), _ct), Times.Once);
    }

    [Fact]
    public async Task DeletePlatformAsync_DeleteEntity()
    {
        // Arrange
        var fakePlatform = _fixture.Create<Data.SQL.Entities.Platform>();
        _mockPlatformRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), _ct))
            .ReturnsAsync(fakePlatform);

        // Act
        await _platformService.DeletePlatformAsync(fakePlatform.Id, _ct);

        // Assert
        _mockPlatformRepository.Verify(x => x.DeleteAsync(fakePlatform, _ct), Times.Once);
    }

    [Fact]
    public async Task GetAllPlatformsAsync_ReturnsAllPlatforms()
    {
        // Arrange
        var fakePlatform = _fixture.CreateMany<Data.SQL.Entities.Platform>();
        _mockPlatformRepository.Setup(x => x.GetAllAsync(_ct))
            .ReturnsAsync(fakePlatform);

        // Act
        var result = await _platformService.GetAllPlatformsAsync(_ct);

        // Assert
        Assert.NotNull(result);
        _mockPlatformRepository.Verify(x => x.GetAllAsync(_ct), Times.Once);
    }

    [Fact]
    public async Task GetPlatformByIdAsync_ReturnSingleValue()
    {
        // Arrange
        var fakePlatform = _fixture.Create<Data.SQL.Entities.Platform>();

        _mockPlatformRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), _ct))
            .ReturnsAsync(fakePlatform);

        // Act
        var genre = await _platformService.GetPlatformByIdAsync(fakePlatform.Id, _ct);

        // Assert
        _mockPlatformRepository.Verify(x => x.GetByIdAsync(fakePlatform.Id, _ct), Times.Once);
        genre.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatePlatformAsync_UpdateEntity()
    {
        // Arrange
        var fakePlatform = _fixture.Create<Data.SQL.Entities.Platform>();
        var fakePlatformDto = _fixture.Create<PlatformDto>();
        _mockPlatformRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), _ct))
            .ReturnsAsync(fakePlatform);

        // Act
        await _platformService.UpdatePlatformAsync(fakePlatformDto, _ct);

        // Assert
        _mockPlatformRepository.Verify(x => x.Update(It.IsAny<Data.SQL.Entities.Platform>()), Times.Once);
    }
}
