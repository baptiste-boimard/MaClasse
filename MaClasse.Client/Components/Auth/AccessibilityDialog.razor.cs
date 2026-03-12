using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.Auth;

public partial class AccessibilityDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

    public void CloseAccessibility()
    {
        MudDialog?.Close(DialogResult.Ok(true));
    }
}