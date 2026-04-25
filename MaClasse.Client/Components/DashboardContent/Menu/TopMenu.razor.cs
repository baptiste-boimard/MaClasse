using MaClasse.Client.Services;
using MaClasse.Client.States;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Menu;

public partial class TopMenu : ComponentBase
{
    private readonly IDialogService _dialogService;
    private readonly ServiceLogout _serviceLogout;
    private readonly UserState _userState;

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
}
