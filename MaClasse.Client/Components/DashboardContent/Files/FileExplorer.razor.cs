using MaClasse.Client.States;
using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MaClasse.Client.Components.DashboardContent.Files;

public partial class FileExplorer : ComponentBase
{
    private readonly LessonState _lessonState;
    private readonly UserState _userState;
    private readonly FileState _fileState;


    public FileExplorer(
        LessonState lessonState,
        UserState userState,
        FileState fileState)
    {
        _lessonState = lessonState;
        _userState = userState;
        _fileState = fileState;
    }

    private Appointment appointement = new Appointment();
    private IList<IBrowserFile> _files = new List<IBrowserFile>();
    
    
    protected override void OnInitialized()
    {
        _lessonState.OnChange += RefreshState;
    }

    private void RefreshState()
    {
        appointement = _lessonState.SelectedAppointment;
        
        InvokeAsync(() => { StateHasChanged(); });
    }
    
    private async Task UploadFiles(IBrowserFile file)
    {
        await _fileState.UploadFile(file);


    }
    
    
    
    
}