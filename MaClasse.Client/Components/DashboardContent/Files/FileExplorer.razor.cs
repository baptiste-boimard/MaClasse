using System.Text;
using MaClasse.Client.States;
using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Files;

public partial class FileExplorer : ComponentBase, IAsyncDisposable
{
    private readonly LessonState _lessonState;
    private readonly IDialogService _dialogService;
    private readonly IJSRuntime _jsRuntime;
    private readonly SchedulerState _schedulerState;


    public FileExplorer(
        LessonState lessonState,
        IDialogService dialogService,
        IJSRuntime jsRuntime,
        SchedulerState schedulerState)
    {
        _lessonState = lessonState;
        _dialogService = dialogService;
        _jsRuntime = jsRuntime;
        _schedulerState = schedulerState;
    }

    private Appointment appointement = new Appointment();
    private List<Document> files = new List<Document>();
    
    private Document selectedDoc;
    private bool showContextMenu;
    private int menuX;
    private int menuY;
    private DotNetObjectReference<FileExplorer>? _dotNetRef;
    private string menuXpx => $"{menuX}px";
    private string menuYpx => $"{menuY}px";
    private bool isReadOnly;
    private bool _isDeleting;
    private string _deleteFileName = string.Empty;
    private bool _horizontalWheelEnabled;
    private string _advancedSearchQuery = string.Empty;
    private const string AdvancedSearchPlaceholder =
        "Décriver le document que vous recherchez,\nex : Trouve les documents traitant de musiques africaines";

    [Parameter] public bool ShowHeader { get; set; } = true;
    [Parameter] public bool ShowFileList { get; set; } = true;
    [Parameter] public string HeaderTitle { get; set; } = "Mes Documents";
    [Parameter] public string HeaderIcon { get; set; } = Icons.Material.Filled.FolderOpen;
    [Parameter] public bool ShowAdvancedSearch { get; set; }
    [Parameter] public EventCallback<string> OnAdvancedSearch { get; set; }
    [Parameter] public bool IsBusy { get; set; }
    
    
    protected override void OnInitialized()
    {
        _lessonState.OnChange += RefreshState;
        SyncFromState();
    }

    private void SyncFromState()
    {
        appointement = _lessonState.SelectedAppointment ?? new Appointment();
        var currentLesson = _lessonState.Lesson;
        files = currentLesson?.Documents ?? new List<Document>();
        isReadOnly = _lessonState.IsReadOnly;
    }

    private void RefreshState()
    {
        SyncFromState();
        InvokeAsync(() => { StateHasChanged(); });
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && ShowFileList)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("documents.setInstance", _dotNetRef);
            await _jsRuntime.InvokeVoidAsync("documents.registerOutsideClick");
        }

        if (ShowFileList && !_horizontalWheelEnabled)
        {
            await _jsRuntime.InvokeVoidAsync("documents.enableHorizontalWheel");
            _horizontalWheelEnabled = true;
        }
    }
    
    private async Task OpenFileInNewTab()
    {
        if (selectedDoc is null)
            return;

        string concatString = $"{_schedulerState.SchedulerDisplayed}-{selectedDoc.IdDocument}";

        var base64EncodedconcatString = Convert.ToBase64String(Encoding.UTF8.GetBytes(concatString));
        
        var viewerUrl = $"/documents/view/{base64EncodedconcatString}";
        
        await _jsRuntime.InvokeVoidAsync("open", viewerUrl, "_blank");
    }
    
    private async Task DeleteFile()
    {
        if (selectedDoc is null || _isDeleting || IsBusy)
        {
            return;
        }

        _isDeleting = true;
        _deleteFileName = selectedDoc.Name ?? "Fichier";
        await InvokeAsync(StateHasChanged);

        try
        {
            await _lessonState.DeleteFileAsync(selectedDoc);
        }
        finally
        {
            await Task.Delay(250);
            _isDeleting = false;
            _deleteFileName = string.Empty;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task RenameFile()
    {
        var parameters = new DialogParameters
        {
            { "Document", selectedDoc }
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
            selectedDoc.Name = newName;
            _lessonState.RenameFile(selectedDoc);
        }
    }
    
    private async Task OnImageClick(MouseEventArgs e, string documentId)
    {
        var x = (int)e.ClientX;
        var y = (int)e.ClientY;
        
        await _jsRuntime.InvokeVoidAsync("documents.handleDocumentClickFromBlazor", documentId, x, y);
    }

    [JSInvokable]
    public async Task ShowDocumentMenu(string id, int x, int y)
    {
        try
        {
            selectedDoc = files.FirstOrDefault(d => d.IdDocument == id);

            if (selectedDoc == null)
            {
                return;
            }

            menuX = x;
            menuY = y;
            showContextMenu = true;

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur dans ShowDocumentMenu: {ex.Message}");
        }
    }


    [JSInvokable]
    public async Task CloseDocumentMenu()
    {
        showContextMenu = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task TriggerAdvancedSearch()
    {
        if (OnAdvancedSearch.HasDelegate)
        {
            await OnAdvancedSearch.InvokeAsync(_advancedSearchQuery);
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        _lessonState.OnChange -= RefreshState;

        if (_horizontalWheelEnabled)
        {
            await _jsRuntime.InvokeVoidAsync("documents.disableHorizontalWheel");
            _horizontalWheelEnabled = false;
        }

        if (_dotNetRef != null)
        {
            _dotNetRef.Dispose();
            _dotNetRef = null;
        }
    }
}
