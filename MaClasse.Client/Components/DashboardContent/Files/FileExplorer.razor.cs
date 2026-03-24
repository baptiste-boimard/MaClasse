using System.Text;
using MaClasse.Client.States;
using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Files;

public partial class FileExplorer : ComponentBase, IAsyncDisposable
{
    private readonly LessonState _lessonState;
    private readonly UserState _userState;
    private readonly IDialogService _dialogService;
    private readonly IJSRuntime _jsRuntime;
    private readonly SchedulerState _schedulerState;


    public FileExplorer(
        LessonState lessonState,
        UserState userState,
        IDialogService dialogService,
        IJSRuntime jsRuntime,
        SchedulerState schedulerState)
    {
        _lessonState = lessonState;
        _userState = userState;
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
    private bool CanUploadFiles => !isReadOnly && !string.IsNullOrWhiteSpace(appointement?.Id);
    private bool _isUploading;
    private bool _isDeleting;
    private int _uploadProgress;
    private string _uploadFileName = string.Empty;
    private string _deleteFileName = string.Empty;
    private string _operationFileName => _isDeleting ? _deleteFileName : _uploadFileName;

    [Parameter] public bool ShowHeader { get; set; } = true;
    [Parameter] public bool ShowUploadButton { get; set; } = true;
    [Parameter] public bool ShowFileList { get; set; } = true;
    
    
    protected override void OnInitialized()
    {
        _lessonState.OnChange += RefreshState;
        
        isReadOnly = _lessonState.IsReadOnly;
    }

    private void RefreshState()
    {
        appointement = _lessonState.SelectedAppointment;
        files = _lessonState.Lesson.Documents;
        isReadOnly = _lessonState.IsReadOnly;

        
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
    }
    
    private async Task UploadFiles(IBrowserFile file)
    {
        if (file is null || _isUploading)
        {
            return;
        }

        _isUploading = true;
        _uploadProgress = 0;
        _uploadFileName = file.Name;
        await InvokeAsync(StateHasChanged);

        var progress = new Progress<int>(value =>
        {
            _uploadProgress = Math.Clamp(value, 0, 100);
            _ = InvokeAsync(StateHasChanged);
        });

        try
        {
            await _lessonState.UploadFileAsync(file, progress);
            _uploadProgress = 100;
        }
        finally
        {
            await Task.Delay(350);
            _isUploading = false;
            _uploadProgress = 0;
            _uploadFileName = string.Empty;
            await InvokeAsync(StateHasChanged);
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
        if (selectedDoc is null || _isDeleting || _isUploading)
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
    
    public async ValueTask DisposeAsync()
    {
        if (_dotNetRef != null)
        {
            _dotNetRef.Dispose();
            _dotNetRef = null;
        }
    }
}
