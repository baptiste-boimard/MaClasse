using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.Errors;

public partial class ErrorLoginDialog : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance? MudDialog { get; set; }
    
    [Parameter]
    public string Message { get; set; }
    
    private void Close()
    {
        MudDialog?.Close(DialogResult.Ok(true));
    }
}