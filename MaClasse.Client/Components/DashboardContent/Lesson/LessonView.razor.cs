using MaClasse.Client.States;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.DashboardContent.Lesson;

public partial class LessonView : ComponentBase
{
    private readonly LessonState _lessonState;
    private readonly HttpClient _httpClient;

    public LessonView(
        LessonState lessonState,
        HttpClient httpClient)
    {
        _lessonState = lessonState;
        _httpClient = httpClient;
    }
    
    private Appointment appointement = new Appointment();
    private Shared.Models.Lesson.Lesson lesson = new Shared.Models.Lesson.Lesson();
    
    
    protected override void OnInitialized()
    {
        _lessonState.OnChange += RefreshState;

    }

    private void RefreshState()
    {
        appointement = _lessonState.SelectedAppointment;
        
        InvokeAsync(() => { StateHasChanged(); });
    }

    private void SaveLesson()
    {
        _lessonState.AddLesson(lesson, appointement);
    }
}