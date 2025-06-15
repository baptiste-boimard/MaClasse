using System.Threading.Tasks;
using Moq;
using Service.OAuth.Interfaces;
using Service.OAuth.Service;
using Xunit;

namespace Tests.Unit;

public class GenerateIdRoleTests
{
    [Fact]
    public async Task GenerateIdAsync_ReturnsAlphanumericString_OfExpectedLength()
    {
        var repoMock = new Mock<IAuthRepository>();
        repoMock.Setup(r => r.CheckIdRole(It.IsAny<string>())).ReturnsAsync(false); // ID libre

        var generator = new GenerateIdRole(repoMock.Object);

        var result = await generator.GenerateIdAsync(8);

        Assert.Equal(8, result.Length);
        Assert.Matches("^[A-Z0-9]{8}$", result);
    }

    [Fact]
    public async Task GenerateIdAsync_ReturnsFirstId_WhenAvailable()
    {
        var repoMock = new Mock<IAuthRepository>();
        repoMock.Setup(r => r.CheckIdRole(It.IsAny<string>())).ReturnsAsync(false);

        var generator = new GenerateIdRole(repoMock.Object);

        var result = await generator.GenerateIdAsync();

        repoMock.Verify(r => r.CheckIdRole(It.IsAny<string>()), Times.Once);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GenerateIdAsync_RetriesWhenIdExists()
    {
        var callCount = 0;
        var repoMock = new Mock<IAuthRepository>();
        repoMock.Setup(r => r.CheckIdRole(It.IsAny<string>()))
                .ReturnsAsync(() => callCount++ >= 2 ? false : true); // 2 collisions

        var generator = new GenerateIdRole(repoMock.Object);

        var result = await generator.GenerateIdAsync();

        repoMock.Verify(r => r.CheckIdRole(It.IsAny<string>()), Times.Exactly(3));
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GenerateIdAsync_GeneratesDifferentIds()
    {
        var repoMock = new Mock<IAuthRepository>();
        repoMock.Setup(r => r.CheckIdRole(It.IsAny<string>())).ReturnsAsync(false);

        var generator = new GenerateIdRole(repoMock.Object);

        var id1 = await generator.GenerateIdAsync();
        var id2 = await generator.GenerateIdAsync();

        Assert.NotEqual(id1, id2);
    }
}
