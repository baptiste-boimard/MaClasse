using MaClasse.Client.States;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Lesson;

public partial class LessonView : ComponentBase
{
    private readonly LessonState _lessonState;
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;

    public LessonView(
        LessonState lessonState,
        HttpClient httpClient,
        IDialogService dialogService)
    {
        _lessonState = lessonState;
        _httpClient = httpClient;
        _dialogService = dialogService;
    }
    
    private Appointment appointement = new Appointment();
    private Shared.Models.Lesson.Lesson lesson = new Shared.Models.Lesson.Lesson();
    private bool isPasteDisabled = true;
    
    protected override void OnInitialized()
    {
        _lessonState.OnChange += RefreshState;
    }

    private async void RefreshState()
    {
        appointement = _lessonState.SelectedAppointment;
        lesson =_lessonState.Lesson;
        
        InvokeAsync(() => { StateHasChanged(); });
    }

    private async void SaveLesson()
    {
        if (appointement.Id != null)
        {
            var resultSave = await _lessonState.AddLesson(lesson, appointement);

            if (resultSave)
            {
                //* Ouverture d'une popup pour confirmation la save
                var parameters = new DialogParameters
                {
                    ["Message"] = "Votre cours a été sauvegardé avec succès !!",
                };
        
                var options = new DialogOptions
                {
                    CloseOnEscapeKey = true,
                    CloseButton = true,
                    MaxWidth = MaxWidth.ExtraSmall,
                    FullWidth = true
                };
        
                var dialog = await _dialogService.ShowAsync<ConfirmSaveLessonDialog>(
                    "Confirmation de Sauvegarde", parameters, options);
        
                var result = await dialog.Result;
            }
        }
    }

    private async void DeleteLesson()
    {
        if (appointement.Id != null)
        {
            //* Ouverture d'une popup de confirmation
            var parameters = new DialogParameters
            {
                ["Message"] = "Êtes-vous sûr de vouloir supprimer ce cour ?",
            };
        
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                CloseButton = true,
                MaxWidth = MaxWidth.ExtraSmall,
                FullWidth = true
            };
        
            var dialog = await _dialogService.ShowAsync<ConfirmDeleteLessonDialog>(
                "Confirmation de suppression", parameters, options);
        
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                _lessonState.DeleteLesson(lesson);
            }
        }
    }

    public void CopyLesson()
    {
        _lessonState.SetCopyLesson(lesson);
        isPasteDisabled = false;
    }
    
    public void PasteLesson()
    {
        lesson = _lessonState.GetCopyLesson();
        isPasteDisabled = true;
    }
}