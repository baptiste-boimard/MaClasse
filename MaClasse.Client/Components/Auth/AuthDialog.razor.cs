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
    public string zoneSelection;
    public bool isClosed = false;
    
    private void TryValidate()
    {
        if (string.IsNullOrWhiteSpace(roleSelection) | string.IsNullOrWhiteSpace(zoneSelection))
        {
            return;
        }
        
        MudDialog?.Close(DialogResult.Ok(new SignupDialogResult
        {
            Role = roleSelection,
            Zone = zoneSelection
        }));

    }
}