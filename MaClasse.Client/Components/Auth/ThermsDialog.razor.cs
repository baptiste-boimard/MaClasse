using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.Auth;

public partial class ThermsDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

    public void CloseTherms()
    {
        Console.WriteLine("→ CloseTherms() appelé"); // ✅ ce log va s’afficher
        MudDialog?.Close(DialogResult.Ok(true));
        Console.WriteLine("→ CloseTherms() appelé");
    }
}