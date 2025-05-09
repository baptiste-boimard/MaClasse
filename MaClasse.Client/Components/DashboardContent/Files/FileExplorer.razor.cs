using MaClasse.Client.States;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.DashboardContent.Files;

public partial class FileExplorer : ComponentBase
{
    private readonly LessonState _lessonState;

    public FileExplorer(LessonState lessonState)
    {
        _lessonState = lessonState;
    }

    private Appointment appointement = new Appointment();
    
    
    protected override void OnInitialized()
    {
        _lessonState.OnChange += RefreshState;
    }

    private void RefreshState()
    {
        appointement = _lessonState.SelectedAppointment;
        
        InvokeAsync(() => { StateHasChanged(); });
    }
    
    
    
    
}