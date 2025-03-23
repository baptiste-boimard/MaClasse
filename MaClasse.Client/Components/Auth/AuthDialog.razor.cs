using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.Auth;

public partial class AuthDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

    [Parameter]
    public UserProfile? User { get; set; }

    public string roleSelection;
    public bool isClosed = false;
    
    private void TryValidate()
    {
        if (string.IsNullOrWhiteSpace(roleSelection))
        {
            return;
        }
        
        MudDialog?.Close(DialogResult.Ok(roleSelection));

    }
}