using Service.Database.Services; 
using Xunit;

namespace Tests.Unit;

public class SlugifyServiceTests
{
  private readonly SlugifyService _service = new();

  [Theory]
  [InlineData("Été Image.PNG", "ete_image")]
  [InlineData("Mon Fichier Avec Accents.docx", "mon_fichier_avec_accents")]
  [InlineData("Résumé Final.pdf", "resume_final")]
  [InlineData("NoAccentFile.JPG", "noaccentfile")]
  [InlineData("Test   Multiple   Spaces.txt", "test___multiple___spaces")]
  [InlineData("Déjà Vu!.mp4", "deja_vu!")]
  
  public void SlugifyFileName_RemovesAccentsAndLowercases(string original, string expected)
  {
    var result = _service.SlugifyFileName(original);
    Assert.Equal(expected, result);
  }

  [Fact]
  public void SlugifyFileName_RemovesExtension()
  {
    var result = _service.SlugifyFileName("FichierAvecExtension.txt");
    Assert.DoesNotContain(".txt", result);
  }

  [Fact]
  public void SlugifyFileName_ReturnsEmpty_OnEmptyString()
  {
    var result = _service.SlugifyFileName("");
    Assert.Equal(string.Empty, result);
  }
}