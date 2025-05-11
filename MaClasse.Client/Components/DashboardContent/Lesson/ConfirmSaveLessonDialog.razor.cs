using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Lesson;

public partial class ConfirmSaveLessonDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

    [Parameter] public string Message { get; set; }

    private void Cancel()
    {
        MudDialog?.Cancel();
    }

    private void Confirm()
    {
        MudDialog?.Close(DialogResult.Ok(true));
    }
}