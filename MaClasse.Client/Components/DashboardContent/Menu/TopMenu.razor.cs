using System.Globalization;
using MaClasse.Client.Services;
using MaClasse.Client.States;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Menu;

public partial class TopMenu : ComponentBase, IDisposable
{
    private readonly IDialogService _dialogService;
    private readonly ServiceLogout _serviceLogout;
    private readonly UserState _userState;
    private readonly IJSRuntime _jsRuntime;
    private readonly CultureInfo _frenchCulture = CultureInfo.GetCultureInfo("fr-FR");
    private System.Timers.Timer? _dateTimeTimer;

    public TopMenu(
        IDialogService dialogService,
        ServiceLogout serviceLogout,
        UserState userState,
        IJSRuntime jsRuntime)
    {
        _dialogService = dialogService;
        _serviceLogout = serviceLogout;
        _userState = userState;
        _jsRuntime = jsRuntime;
    }

    public string Picture => _userState.Picture;
    public string Email => _userState.Email;
    private string CurrentDateTimeAriaLabel { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        UpdateCurrentDateTimeAriaLabel();

        _dateTimeTimer = new System.Timers.Timer(1000);
        _dateTimeTimer.Elapsed += (_, _) =>
        {
            UpdateCurrentDateTimeAriaLabel();
            InvokeAsync(StateHasChanged);
        };
        _dateTimeTimer.Start();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await _jsRuntime.InvokeVoidAsync(
            "focusHelpers.wireTabSequenceByIds",
            (object)new[]
            {
                "view-dashboard-current-entry",
                "top-menu-dashboard-select",
                "top-menu-date-time",
                "top-menu-avatar-button",
                "top-menu-mail-link",
                "top-menu-button-logout",
            });
    }

    public async Task OpenProfileDialog()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
        };

        var dialog = await _dialogService.ShowAsync<ProfileDialog>("", options);
        await dialog.Result;

        StateHasChanged();
    }

    public async Task Logout()
    {
        await _serviceLogout.Logout(_userState.IdSession);
    }

    private void UpdateCurrentDateTimeAriaLabel()
    {
        var now = TimeZoneInfo.ConvertTime(
            DateTimeOffset.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));

        CurrentDateTimeAriaLabel =
            $"Date et heure actuelles : {now.ToString("dddd d MMMM yyyy 'à' HH:mm", _frenchCulture)}";
    }

    public void Dispose()
    {
        _dateTimeTimer?.Stop();
        _dateTimeTimer?.Dispose();
    }
}
