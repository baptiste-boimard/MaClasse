using System.Text;
using MaClasse.Client.States;
using MaClasse.Shared.Models.Files;
using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.Pages;

public partial class DocumentView : ComponentBase
{
  private readonly LessonState _lessonState;
  private readonly HttpClient _httpClient;

  public DocumentView(
    LessonState lessonState,
    HttpClient httpClient)
  {
    _lessonState = lessonState;
    _httpClient = httpClient;
  }

  private static readonly string[] ImageFormats = { "png", "jpg", "jpeg", "bmp", "gif", "webp", "image/png", "image/jpeg" };
  private static readonly string[] PdfFormats = { "pdf", "application/pdf" };

  private bool IsImage(string format)
    => !string.IsNullOrEmpty(format) && ImageFormats.Any(f => f.Equals(format, StringComparison.OrdinalIgnoreCase));

  // private bool IsPdf(string format)
  //   => !string.IsNullOrEmpty(format) && PdfFormats.Any(f => f.Equals(format, StringComparison.OrdinalIgnoreCase));
  private bool IsPdf(string format) =>
    !string.IsNullOrEmpty(format) && 
    (format.Equals("pdf", StringComparison.OrdinalIgnoreCase) ||
     format.Equals("application/pdf", StringComparison.OrdinalIgnoreCase));

  
  [Parameter] public string ConcatString { get; set; }

  private Document document;
  private bool isLoading = true;

  private string PdfViewUrl
  {
    get
    {
      if (document?.Url == null)
      {
        return null;
      }
      // On ajoute #toolbar=0 pour cacher la barre d'outils
      // et #navpanes=0 pour cacher le panneau latéral (pour la compatibilité)
      // return $"{document.Url}#toolbar=0&navpanes=0";
      return $"{document.Url}";
    }
  }

  protected override async Task OnInitializedAsync()
  {
    await LoadDocumentAsync();
  }
  
  private async Task LoadDocumentAsync()
  {
    isLoading = true;
    document = null;
    
    try
    {

      var decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(ConcatString)).Split("-");
      
      document = await _lessonState.GetDocument(decodedString[0], decodedString[1]);

    }
    catch (Exception ex)
    {
      Console.WriteLine($"Exception loading document: {ex.Message}");
    }
    finally
    {
      isLoading = false;
      await InvokeAsync(StateHasChanged);
    }
  }
  
  
  
}