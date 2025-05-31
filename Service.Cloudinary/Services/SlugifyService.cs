using System.Globalization;
using System.Text;

namespace Service.Database.Services;

public class SlugifyService
{
  public string SlugifyFileName(string name)
  {
    return Path.GetFileNameWithoutExtension(name)
      .Normalize(NormalizationForm.FormD)                          // décompose les accents
      .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) // supprime les accents
      .Aggregate("", (a, c) => a + c)
      .Replace(" ", "_")
      .ToLowerInvariant();
  }
}