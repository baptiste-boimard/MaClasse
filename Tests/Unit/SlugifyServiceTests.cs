using Service.Database.Services;
using Xunit;

namespace Tests.Unit;

public class SlugifyServiceTests
{
    [Theory]
    [InlineData("Été Document.pdf", "ete_document")]
    [InlineData("Test File.PNG", "test_file")]
    public void SlugifyFileName_ReturnsSlugged(string input, string expected)
    {
        var service = new SlugifyService();
        var result = service.SlugifyFileName(input);
        Assert.Equal(expected, result);
    }
}

