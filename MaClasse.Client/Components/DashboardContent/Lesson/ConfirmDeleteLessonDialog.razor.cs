using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Lesson;

public partial class ConfirmDeleteLessonDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

    [Parameter] public string Message { get; set; } = "Êtes-vous sûr de vouloir supprimer cet élément ?";

    private void Cancel()
    {
        MudDialog?.Cancel();
    }

    private void Confirm()
    {
        MudDialog?.Close(DialogResult.Ok(true));
    }
}