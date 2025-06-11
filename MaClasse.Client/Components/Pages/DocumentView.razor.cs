using System.Text;
using MaClasse.Client.States;
using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Lesson;
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
  
  [Parameter] public string ConcatString { get; set; }

  private Document document;
  private bool isLoading = true;

  protected async override void OnInitialized()
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
      
      // Ici, vous faites un appel API pour récupérer le document par son ID
      // Assurez-vous d'avoir un endpoint API qui retourne un Document par ID.
      // Exemple : GET /api/documents/{documentId}
      document = await _lessonState.GetDocument(decodedString[0], decodedString[1]);

      // if (response.IsSuccessStatusCode)
      // {
      //   document = await response.Content.ReadFromJsonAsync<Document>();
      //   // Optionnel : si le document vient d'être chargé, mettre à jour le LessonState
      //   // Cela pourrait être utile si d'autres composants s'attendent à ce que LessonState contienne ce document.
      //   // _lessonState.AddOrUpdateDocument(document); // Vous devrez implémenter cette méthode dans LessonState
      // }
      // else
      // {
      //   Console.WriteLine($"Error loading document: {response.StatusCode} - {response.ReasonPhrase}");
      // }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Exception loading document: {ex.Message}");
    }
    finally
    {
      isLoading = false;
      await InvokeAsync(StateHasChanged); // Forcer le rafraîchissement de l'UI
    }
  }
  
  
  
}