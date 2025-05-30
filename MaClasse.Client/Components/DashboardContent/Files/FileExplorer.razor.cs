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


    public FileExplorer(
        LessonState lessonState,
        UserState userState)
    {
        _lessonState = lessonState;
        _userState = userState;
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
    
    
    
    
}