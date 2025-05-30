using MaClasse.Shared.Models.Files;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Files;

public partial class RenameDocumentDialog : ComponentBase
{
  [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

  // [Parameter] public string Message { get; set; }
  [Parameter] public Document Document { get; set; }
  
  private string NewName { get; set; } = String.Empty;

  private void Cancel()
  {
    MudDialog?.Cancel();
  }

  private void Confirm()
  {
    MudDialog?.Close(DialogResult.Ok(NewName));
  }
}