using Service.Database.Services;
using Xunit;

namespace MaClasse.Tests.Services;

public class SlugifyServiceTests
{
    [Fact]
    public void SlugifyFileName_ReturnsSlugifiedName()
    {
        var service = new SlugifyService();
        var result = service.SlugifyFileName("Épreuve finale.pdf");
        Assert.Equal("epreuve_finale", result);
    }

    [Fact]
    public void SlugifyFileName_WithEmptyString_ReturnsEmptyString()
    {
        var service = new SlugifyService();
        var result = service.SlugifyFileName(string.Empty);
        Assert.Equal(string.Empty, result);
    }
}
