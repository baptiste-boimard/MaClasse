using System.Globalization;
using MaClasse.Client.Services;
using MaClasse.Client.States;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Menu;

public partial class TopMenu : ComponentBase, IDisposable
{
    private readonly IDialogService _dialogService;
    private readonly ServiceLogout _serviceLogout;
    private readonly UserState _userState;
    private readonly CultureInfo _frenchCulture = CultureInfo.GetCultureInfo("fr-FR");
    private System.Timers.Timer? _dateTimeTimer;

    public TopMenu(
        IDialogService dialogService,
        ServiceLogout serviceLogout,
        UserState userState)
    {
        _dialogService = dialogService;
        _serviceLogout = serviceLogout;
        _userState = userState;
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
