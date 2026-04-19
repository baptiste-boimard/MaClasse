using System.Text;
using MaClasse.Client.States;
using System.Globalization;
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
    private readonly ViewDashboardState _viewDashboardState;
    private readonly ISnackbar _snackbar;


    public FileExplorer(
        LessonState lessonState,
        IDialogService dialogService,
        IJSRuntime jsRuntime,
        SchedulerState schedulerState,
        ViewDashboardState viewDashboardState,
        ISnackbar snackbar)
    {
        _lessonState = lessonState;
        _dialogService = dialogService;
        _jsRuntime = jsRuntime;
        _schedulerState = schedulerState;
        _viewDashboardState = viewDashboardState;
        _snackbar = snackbar;
    }

    private Appointment appointement = new Appointment();
    private List<Document> files = new List<Document>();
    
    private Document? selectedDoc;
    private bool showContextMenu;
    private int menuX;
    private int menuY;
    private DotNetObjectReference<FileExplorer>? _dotNetRef;
    private string menuXpx => $"{menuX}px";
    private string menuYpx => $"{menuY}px";
    private bool isReadOnly;
    private bool _isDeleting;
    private string _deleteFileName = string.Empty;
    private bool _menuFromAdvancedSearch;
    private List<Appointment> _advancedSearchLessonChoices = new();
    private bool _horizontalWheelEnabled;
    private string _advancedSearchQuery = string.Empty;
    private List<Document> _advancedSearchResults = new();
    private bool _isAdvancedSearchLoading;
    private bool _hasAdvancedSearchCompleted;
    private string _lastCompletedAdvancedSearchQuery = string.Empty;
    private string _selectedDocumentId = string.Empty;
    private bool _focusContextMenuRequested;
    private readonly string _contextMenuId = $"file-explorer-context-menu-{Guid.NewGuid():N}";
    private readonly string _firstContextMenuItemId = $"file-explorer-context-menu-first-item-{Guid.NewGuid():N}";
    private MudTextField<string>? _advancedSearchInputRef;
    private bool _moveFocusAfterAdvancedSearch;
    private bool _focusFirstAdvancedSearchResultAfterRender;
    private bool _focusAdvancedSearchFallbackAfterRender;
    private bool IsViewingAnotherDashboard =>
        !string.IsNullOrWhiteSpace(_schedulerState.SchedulerDisplayed) &&
        !string.Equals(_schedulerState.SchedulerDisplayed, _schedulerState.IdUser, StringComparison.Ordinal);
    private const string AdvancedSearchPlaceholder =
        "Décriver le document que vous recherchez,\nex : Trouve les documents traitant de musiques africaines";

    private string AdvancedSearchQuery
    {
        get => _advancedSearchQuery;
        set
        {
            if (_advancedSearchQuery == value)
            {
                return;
            }

            _advancedSearchQuery = value;
            _hasAdvancedSearchCompleted = false;
        }
    }

    private string AdvancedSearchAriaStatus
    {
        get
        {
            if (_isAdvancedSearchLoading)
            {
                return "Recherche en cours";
            }

            if (_hasAdvancedSearchCompleted)
            {
                return _advancedSearchResults.Count > 0
                    ? $"{_advancedSearchResults.Count} document(s) trouvé(s)"
                    : "Aucun document trouvé";
            }

            return string.Empty;
        }
    }

    [Parameter] public bool ShowHeader { get; set; } = true;
    [Parameter] public bool ShowFileList { get; set; } = true;
    [Parameter] public string HeaderTitle { get; set; } = "Mes Documents";
    [Parameter] public string HeaderIcon { get; set; } = Icons.Material.Filled.FolderOpen;
    [Parameter] public bool ShowAdvancedSearch { get; set; }
    [Parameter] public bool AllowAdvancedSearchFocus { get; set; } = true;
    [Parameter] public bool IsBusy { get; set; }
    [Parameter] public string BusyFileName { get; set; } = string.Empty;
    [Parameter] public EventCallback OnAdvancedSearchNoResultFocusFallback { get; set; }
    
    
    protected override void OnInitialized()
    {
        _lessonState.OnChange += RefreshState;
        _schedulerState.OnChange += RefreshState;
        SyncFromState();
    }

    private void SyncFromState()
    {
        appointement = _lessonState.SelectedAppointment ?? new Appointment();
        var currentLesson = _lessonState.Lesson;
        files = currentLesson?.Documents ?? new List<Document>();
        isReadOnly = _lessonState.IsReadOnly;

        if (!string.IsNullOrWhiteSpace(_selectedDocumentId) &&
            !files.Any(d => string.Equals(d.IdDocument, _selectedDocumentId, StringComparison.Ordinal)) &&
            !_advancedSearchResults.Any(d => string.Equals(d.IdDocument, _selectedDocumentId, StringComparison.Ordinal)))
        {
            _selectedDocumentId = string.Empty;
            selectedDoc = null;
        }
    }

    private void RefreshState()
    {
        SyncFromState();
        InvokeAsync(() => { StateHasChanged(); });
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && (ShowFileList || ShowAdvancedSearch))
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("documents.setInstance", _dotNetRef);
            await _jsRuntime.InvokeVoidAsync("documents.registerOutsideClick");
        }

        if (showContextMenu && _focusContextMenuRequested)
        {
            _focusContextMenuRequested = false;
            await _jsRuntime.InvokeVoidAsync(
                "documents.focusContextMenuFirstItem",
                _firstContextMenuItemId,
                _contextMenuId);
            await _jsRuntime.InvokeVoidAsync(
                "documents.watchFocusOutsideMenu",
                _contextMenuId,
                _dotNetRef);
        }

        if (_focusFirstAdvancedSearchResultAfterRender && _advancedSearchResults.Count > 0)
        {
            _focusFirstAdvancedSearchResultAfterRender = false;
            await _jsRuntime.InvokeVoidAsync("documents.focusElementById", GetAdvancedSearchResultCardId(0));
        }

        if (_focusAdvancedSearchFallbackAfterRender)
        {
            _focusAdvancedSearchFallbackAfterRender = false;
            if (OnAdvancedSearchNoResultFocusFallback.HasDelegate)
            {
                await OnAdvancedSearchNoResultFocusFallback.InvokeAsync();
            }
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
        var docToRename = selectedDoc;
        if (docToRename is null)
            return;

        var parameters = new DialogParameters
        {
            { "Document", docToRename }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small
        };

        var dialog = await _dialogService.ShowAsync<RenameDocumentDialog>(
            "Renommer votre fichier", parameters, options);

        if (dialog is null)
            return;

        var result = await dialog.Result;

        if (result is null || result.Canceled || result.Data is not string newName || string.IsNullOrWhiteSpace(newName))
            return;

        docToRename.Name = newName;
        _lessonState.RenameFile(docToRename);
    }
    
    private async Task OnImageClick(MouseEventArgs e, string documentId)
    {
        var x = (int)e.ClientX;
        var y = (int)e.ClientY;

        _selectedDocumentId = documentId ?? string.Empty;
        selectedDoc = files.FirstOrDefault(d => d.IdDocument == _selectedDocumentId) ??
                      _advancedSearchResults.FirstOrDefault(d => d.IdDocument == _selectedDocumentId);
        await InvokeAsync(StateHasChanged);
        
        await _jsRuntime.InvokeVoidAsync("documents.handleDocumentClickFromBlazor", documentId, x, y);
    }

    private async Task OnAdvancedSearchImageClick(MouseEventArgs e, Document doc)
    {
        await OpenDocumentMenuAsync(doc, fromAdvancedSearch: true, (int)e.ClientX, (int)e.ClientY);
    }

    private async Task OnFileCardKeyDown(KeyboardEventArgs e, Document doc)
    {
        if (e.Key is "Enter" or " ")
        {
            var pos = await _jsRuntime.InvokeAsync<MenuPosition>("documents.getActiveElementMenuPosition");
            await OpenDocumentMenuAsync(doc, fromAdvancedSearch: false, pos.X, pos.Y);
        }
    }

    private async Task OnAdvancedSearchCardKeyDown(KeyboardEventArgs e, Document doc)
    {
        if (e.Key is "Enter" or " ")
        {
            var pos = await _jsRuntime.InvokeAsync<MenuPosition>("documents.getActiveElementMenuPosition");
            await OpenDocumentMenuAsync(doc, fromAdvancedSearch: true, pos.X, pos.Y);
        }
    }

    private sealed record MenuPosition(int X, int Y);

    private async Task HandleMenuBackdropKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            await CloseDocumentMenu();
        }
    }

    private async Task HandleContextMenuKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            await CloseDocumentMenu();
        }
    }

    [JSInvokable]
    public async Task ShowDocumentMenu(string id, int x, int y)
    {
        try
        {
            _selectedDocumentId = id ?? string.Empty;
            selectedDoc = files.FirstOrDefault(d => d.IdDocument == id);
            selectedDoc ??= _advancedSearchResults.FirstOrDefault(d => d.IdDocument == id);
            _menuFromAdvancedSearch = !files.Any(d => d.IdDocument == id);
            _advancedSearchLessonChoices.Clear();

            if (selectedDoc == null)
            {
                return;
            }

            menuX = x;
            menuY = y;
            showContextMenu = true;
            _focusContextMenuRequested = true;

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
        await _jsRuntime.InvokeVoidAsync("documents.cancelFocusOutsideMenu");

        var hasChanged = showContextMenu ||
                         _menuFromAdvancedSearch ||
                         _advancedSearchLessonChoices.Count > 0;

        showContextMenu = false;
        _menuFromAdvancedSearch = false;
        _advancedSearchLessonChoices.Clear();
        hasChanged |= ClearDocumentSelectionState();

        if (hasChanged)
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    [JSInvokable]
    public async Task CloseMenuOnFocusOut()
    {
        await CloseDocumentMenu();
    }

    [JSInvokable]
    public async Task ClearDocumentSelection()
    {
        if (ClearDocumentSelectionState())
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task OpenDocumentMenuAsync(Document? doc, bool fromAdvancedSearch, int x, int y)
    {
        if (doc is null)
        {
            return;
        }

        selectedDoc = doc;
        _selectedDocumentId = doc.IdDocument ?? string.Empty;
        _menuFromAdvancedSearch = fromAdvancedSearch;
        _advancedSearchLessonChoices.Clear();
        menuX = x;
        menuY = y;
        showContextMenu = true;
        _focusContextMenuRequested = true;

        await InvokeAsync(StateHasChanged);
    }

    private static string GetDocumentCardAriaLabel(Document doc)
    {
        var docName = string.IsNullOrWhiteSpace(doc.Name) ? "document sans nom" : doc.Name;
        return $"Ouvrir le menu du document {docName}";
    }

    private static string GetAdvancedSearchResultCardId(int index) => $"file-explorer-advanced-result-{index}";

    private bool ClearDocumentSelectionState()
    {
        var hasChanged = false;

        if (!string.IsNullOrWhiteSpace(_selectedDocumentId))
        {
            _selectedDocumentId = string.Empty;
            hasChanged = true;
        }

        if (selectedDoc is not null)
        {
            selectedDoc = null;
            hasChanged = true;
        }

        return hasChanged;
    }

    private string GetFileCardClass(Document doc)
    {
        if (doc is null)
        {
            return "file-explorer-card";
        }

        var isSelected = !string.IsNullOrWhiteSpace(doc.IdDocument) &&
                         string.Equals(doc.IdDocument, _selectedDocumentId, StringComparison.Ordinal);

        return isSelected
            ? "file-explorer-card is-selected"
            : "file-explorer-card";
    }

    private int GetAdvancedSearchTabIndex() => AllowAdvancedSearchFocus ? 0 : -1;

    private string GetLessonChoiceLabel(Appointment appointment)
    {
        var text = string.IsNullOrWhiteSpace(appointment.Text) ? "Leçon" : appointment.Text;
        return $"{text} - {appointment.Start.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}";
    }

    private string GetLessonChoiceAriaLabel(Appointment appointment)
    {
        var text = string.IsNullOrWhiteSpace(appointment.Text) ? "Leçon" : appointment.Text;
        var frenchCulture = CultureInfo.GetCultureInfo("fr-FR");
        var fullDate = appointment.Start.ToString("dddd d MMMM yyyy 'à' HH:mm", frenchCulture);
        return $"{text}, {fullDate}";
    }

    private async Task OpenLessonChoice(Appointment appointment)
    {
        _lessonState.SetLessonSelected(appointment);
        await CloseDocumentMenu();
    }

    private async Task OpenLessonFromAdvancedSearch()
    {
        if (selectedDoc is null)
        {
            return;
        }

        var references = await _lessonState.GetLessonReferencesByDocumentAsync(selectedDoc);
        if (references.Count == 0)
        {
            _snackbar.Add("Aucune leçon trouvée pour ce document.", Severity.Info);
            return;
        }

        var appointmentIds = references
            .Select(r => r.IdAppointment)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToHashSet();

        var sourceAppointments = _schedulerState.Appointments ?? new List<Appointment>();
        if (IsViewingAnotherDashboard)
        {
            sourceAppointments = _viewDashboardState.DashBoards?
                .FirstOrDefault(d => d.UserId == _schedulerState.SchedulerDisplayed)?
                .UserScheduler?
                .Appointments ?? sourceAppointments;
        }

        var matchingAppointments = sourceAppointments
            .Where(a => a.Id is not null && appointmentIds.Contains(a.Id))
            .OrderBy(a => a.Start)
            .ToList();

        if (matchingAppointments.Count == 0)
        {
            _snackbar.Add("Impossible de retrouver les leçons dans l'agenda courant.", Severity.Warning);
            return;
        }

        if (matchingAppointments.Count == 1)
        {
            _lessonState.SetLessonSelected(matchingAppointments[0]);
            await CloseDocumentMenu();
            return;
        }

        _advancedSearchLessonChoices = matchingAppointments;
        await InvokeAsync(StateHasChanged);
    }

    private async Task CopyToCurrentLessonFromAdvancedSearch()
    {
        if (IsViewingAnotherDashboard)
        {
            return;
        }

        if (selectedDoc is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_lessonState.SelectedAppointment?.Id))
        {
            _snackbar.Add("Une leçon doit être sélectionnée", Severity.Warning);
            return;
        }

        var copied = await _lessonState.CopyDocumentToCurrentLessonAsync(selectedDoc);
        if (!copied)
        {
            _snackbar.Add("Impossible de copier le document dans la leçon ouverte.", Severity.Error);
            return;
        }

        _snackbar.Add("Document copié dans la leçon.", Severity.Success);
        await CloseDocumentMenu();
    }

    private async Task TriggerAdvancedSearch()
    {
        _hasAdvancedSearchCompleted = false;
        _isAdvancedSearchLoading = true;
        await InvokeAsync(StateHasChanged);

        try
        {
            _advancedSearchResults = await _lessonState.SearchAdvancedDocumentsAsync(_advancedSearchQuery);
        }
        finally
        {
            _isAdvancedSearchLoading = false;
            _hasAdvancedSearchCompleted = true;
            _lastCompletedAdvancedSearchQuery = _advancedSearchQuery;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task OnAdvancedSearchButtonClick()
    {
        var shouldMoveFocus = _moveFocusAfterAdvancedSearch;
        _moveFocusAfterAdvancedSearch = false;

        await TriggerAdvancedSearch();

        if (!shouldMoveFocus)
        {
            return;
        }

        if (_advancedSearchResults.Count > 0)
        {
            _focusFirstAdvancedSearchResultAfterRender = true;
        }
        else
        {
            _focusAdvancedSearchFallbackAfterRender = true;
        }

        await InvokeAsync(StateHasChanged);
    }

    private void HandleAdvancedSearchButtonKeyDown(KeyboardEventArgs e)
    {
        if (e.Key is "Enter" or " " or "Space" or "Spacebar")
        {
            _moveFocusAfterAdvancedSearch = true;
        }
    }

    private async Task HandleAdvancedSearchKeyDown(KeyboardEventArgs e)
    {
        if (!AllowAdvancedSearchFocus)
        {
            return;
        }

        if (e.Key == "Enter" && !e.ShiftKey)
        {
            await TriggerAdvancedSearch();
        }
    }

    public async Task FocusAdvancedSearchInputAsync()
    {
        if (!ShowAdvancedSearch || !AllowAdvancedSearchFocus || _advancedSearchInputRef is null)
        {
            return;
        }

        await _advancedSearchInputRef.FocusAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        _lessonState.OnChange -= RefreshState;
        _schedulerState.OnChange -= RefreshState;

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
