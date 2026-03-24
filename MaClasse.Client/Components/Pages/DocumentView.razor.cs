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
    IJSRuntime jsRuntime,
    ILogger<DocumentView> logger)
  {
    _lessonState = lessonState;
    _jsRuntime = jsRuntime;
    _logger = logger;
  }

  private static readonly string[] ImageFormats = { "png", "jpg", "jpeg", "bmp", "gif", "webp", "image/png", "image/jpeg" };

  [Parameter] public string ConcatString { get; set; } = string.Empty;

  private Document? document;
  private bool isLoading = true;

  private bool IsImage(string? format)
    => !string.IsNullOrEmpty(format) && ImageFormats.Any(f => f.Equals(format, StringComparison.OrdinalIgnoreCase));

  private bool IsPdf(string? format) =>
    !string.IsNullOrEmpty(format) &&
    (format.Equals("pdf", StringComparison.OrdinalIgnoreCase) ||
     format.Equals("application/pdf", StringComparison.OrdinalIgnoreCase));

  private string? PdfViewUrl => document?.Url;

  private double currentZoom = 1.0;
  private const double ZOOM_STEP = 0.1;
  private const double MAX_ZOOM = 2.0;
  private const double MIN_ZOOM = 0.5;

  private ElementReference documentContainerRef;
  private bool isFullscreen;

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
      _logger.LogInformation("Document chargé: {DocumentName}, ID: {DocumentId}", document?.Name, document?.IdDocument);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Erreur de chargement du document.");
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

  private async Task CloseDocumentView()
  {
    await _jsRuntime.InvokeVoidAsync("closeCurrentTab");
  }
}
