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
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("documents.setInstance", _dotNetRef);
            await _jsRuntime.InvokeVoidAsync("documents.registerOutsideClick");
        }
    }
    
    private async Task UploadFiles(IBrowserFile file)
    {
        _lessonState.UploadFile(file);
    }

    private async Task OpenFileInNewTab()
    {
        // await _jsRuntime.InvokeVoidAsync("open", selectedDoc.Url, "_blank");
        
        if (selectedDoc is null)
            return;

        string concatString = $"{_schedulerState.IdUser}-{selectedDoc.IdDocument}";

        var base64EncodedconcatString = Convert.ToBase64String(Encoding.UTF8.GetBytes(concatString));
        
        var viewerUrl = $"/documents/view/{base64EncodedconcatString}";
        await _jsRuntime.InvokeVoidAsync("open", viewerUrl, "_blank");
    }
    
    private async Task DeleteFile()
    {
        _lessonState.DeleteFile(selectedDoc);
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
    
    //* Ouuverture du menu quand clic sur un document
    // [JSInvokable("ShowCustomMenu")]
    // public async Task ShowCustomMenu(string id, int x, int y)
    // {
    //     selectedAppointment = _schedulerState.Appointments.FirstOrDefault(a => a.Id == id);
    //     menuX = x;
    //     menuY = y;
    //     showContextMenu = true;
    //
    //     await InvokeAsync(StateHasChanged);
    // }
    //
    // [JSInvokable]
    // public async void CloseCustomMenu()
    // {
    //     isClosingContextMenu = true;
    //
    //     await Task.Delay(150);
    //     showContextMenu = false;
    //     isClosingContextMenu = false;
    //
    //     await InvokeAsync(StateHasChanged);
    // }
    
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
                Console.WriteLine($"❌ Aucun document trouvé avec l'ID {id}");
                return;
            }

            menuX = x;
            menuY = y;
            showContextMenu = true;

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur dans ShowDocumentMenu: {ex.Message}");
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