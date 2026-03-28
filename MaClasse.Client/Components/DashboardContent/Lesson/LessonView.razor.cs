using MaClasse.Client.States;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
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
    private bool isReadOnly;
    private int activeLessonTabIndex;
    private bool _isUploading;
    private string _uploadFileName = string.Empty;
    private bool CanUploadFiles => !isReadOnly && !string.IsNullOrWhiteSpace(appointement?.Id);

    private string GetTabButtonClass(int tabIndex)
    {
        return activeLessonTabIndex == tabIndex
            ? "lesson-tab-button is-active"
            : "lesson-tab-button";
    }

    private void HandleTabKeyDown(KeyboardEventArgs e, int currentIndex)
    {
        if (e.Key == "ArrowRight")
        {
            activeLessonTabIndex = (currentIndex + 1) % 4;
        }
        else if (e.Key == "ArrowLeft")
        {
            activeLessonTabIndex = (currentIndex + 3) % 4;
        }
        else if (e.Key == "Home")
        {
            activeLessonTabIndex = 0;
        }
        else if (e.Key == "End")
        {
            activeLessonTabIndex = 3;
        }
    }

    protected override void OnInitialized()
    {
        _lessonState.OnChange += RefreshState;

        isReadOnly = _lessonState.IsReadOnly;
    }

    private async void RefreshState()
    {
        appointement = _lessonState.SelectedAppointment ?? new Appointment();
        lesson = _lessonState.Lesson ?? new Shared.Models.Lesson.Lesson();
        isReadOnly = _lessonState.IsReadOnly;

        InvokeAsync(() => { StateHasChanged(); });
    }

    private async Task UploadFiles(IBrowserFile file)
    {
        if (file is null || _isUploading)
        {
            return;
        }

        _uploadFileName = file.Name ?? string.Empty;
        _isUploading = true;
        await InvokeAsync(StateHasChanged);

        try
        {
            await _lessonState.UploadFileAsync(file);
        }
        finally
        {
            await Task.Delay(350);
            _isUploading = false;
            _uploadFileName = string.Empty;
            await InvokeAsync(StateHasChanged);
        }
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
