using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.Auth;

public partial class PolicyDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

    public void ClosePolicy()
    {
        MudDialog?.Close(DialogResult.Ok(true));
    }
}