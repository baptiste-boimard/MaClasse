using MaClasse.Client.States;
using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Files;

public partial class FileExplorer : ComponentBase
{
    private readonly LessonState _lessonState;
    private readonly UserState _userState;
    private readonly IDialogService _dialogService;


    public FileExplorer(
        LessonState lessonState,
        UserState userState,
        IDialogService dialogService)
    {
        _lessonState = lessonState;
        _userState = userState;
        _dialogService = dialogService;
    }

    private Appointment appointement = new Appointment();
    private List<Document> files = new List<Document>();
    
    
    protected override void OnInitialized()
    {
        _lessonState.OnChange += RefreshState;
    }

    private void RefreshState()
    {
        appointement = _lessonState.SelectedAppointment;
        files = _lessonState.Lesson.Documents;
        
        InvokeAsync(() => { StateHasChanged(); });
    }
    
    private async Task UploadFiles(IBrowserFile file)
    {
        _lessonState.UploadFile(file);
    }

    private async Task DeleteFile(Document document)
    {
        _lessonState.DeleteFile(document);
    }

    private async Task RenameFile(Document document)
    {
        var parameters = new DialogParameters
        {
            // { "Message", "Renommez votre fichier" },
            { "Document", document }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small
        };

        var dialog = await _dialogService.ShowAsync<RenameDocumentDialog>(
            "Renommer votre fichier", parameters, options);

        var result = await dialog.Result;

        if (!result.Canceled && result.Data is string newName && !string.IsNullOrWhiteSpace(newName))
        {
            document.Name = newName;
            _lessonState.RenameFile(document);
        }
    }
    
    
    
    
}