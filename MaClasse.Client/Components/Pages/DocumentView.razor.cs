using System.Text;
using MaClasse.Client.States;
using MaClasse.Shared.Models.Files;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MaClasse.Client.Components.Pages;

public partial class DocumentView : ComponentBase
{
  private readonly LessonState _lessonState;
  private readonly IJSRuntime _jsRuntime;
  private readonly ILogger<DocumentView> _logger;

  public DocumentView(
    LessonState lessonState,
    IJSRuntime jsRuntime, ILogger<DocumentView> logger)
  {
    _lessonState = lessonState;
    _jsRuntime = jsRuntime;
    _logger = logger;
  }
  
  // essais 5
  private static readonly string[] ImageFormats = { "png", "jpg", "jpeg", "bmp", "gif", "webp", "image/png", "image/jpeg" };

  private bool IsImage(string format)
    => !string.IsNullOrEmpty(format) && ImageFormats.Any(f => f.Equals(format, StringComparison.OrdinalIgnoreCase));
  
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
      if (document?.Url == null) return null;
            
      //? Info
      //? On ajoute #toolbar=0 pour cacher la barre d'outils
      //? et #navpanes=0 pour cacher le panneau latéral (pour la compatibilité)
      //? return $"{document.Url}#toolbar=0&navpanes=0";
      
      return $"{document.Url}";
    }
  }
  
  //* Propriétés pour le zoom
  private double currentZoom = 1.0;
  private const double ZOOM_STEP = 0.1; 
  private const double MAX_ZOOM = 2.0; 
  private const double MIN_ZOOM = 0.5; 
  
  //* Propriétés pour le plein écran
  private ElementReference documentContainerRef;
  private bool isFullscreen = false; 

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
      
      _logger.LogInformation("dsfsdfsdfdsfdsf: ", decodedString);

      
      document = await _lessonState.GetDocument(decodedString[0], decodedString[1]);

      _logger.LogInformation("Document '' (ID : ) récupéré avec succès. URL: ", document.Name, document.IdDocument, document.Url);

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
  
  private async Task ToggleFullscreen()
  {
    if (documentContainerRef.Id == null) return;

    if (!isFullscreen)
    {
      await _jsRuntime.InvokeVoidAsync("requestFullscreen", documentContainerRef);
      isFullscreen = true;
    }
    else
    {
      await _jsRuntime.InvokeVoidAsync("exitFullscreen");
      isFullscreen = false;
    }
  }
  
  private void ZoomIn()
  {
    double newZoom = Math.Min(currentZoom + ZOOM_STEP, MAX_ZOOM);
    currentZoom = Math.Round(newZoom, 2); 
    StateHasChanged();
    
  }

  private void ZoomOut()
  {
    double newZoom = Math.Max(currentZoom - ZOOM_STEP, MIN_ZOOM);
    currentZoom = Math.Round(newZoom, 2); 
    StateHasChanged();
    
  }

  private void ResetZoom()
  {
    currentZoom = 1.0;
    StateHasChanged();

  }

  private async void CloseDocumentView()
  {
    await _jsRuntime.InvokeVoidAsync("closeCurrentTab");

  }
}