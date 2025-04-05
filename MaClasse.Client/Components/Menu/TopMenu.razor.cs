using MaClasse.Client.Services;
using MaClasse.Client.States;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.Menu;

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
    [Parameter] public string Picture { get; set; }
    [Parameter] public string Email { get; set; }

    public async Task OpenProfileDialog()
    {
        //* Options de la boîte de dialogue : fermeture sur Esc ou clic en dehors
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            FullWidth = true,         
            MaxWidth = MaxWidth.Small,
        };
        
        //* Affichage de la boîte de dialogue
        var dialog = await _dialogService.ShowAsync<ProfileDialog>("", options);
        await dialog.Result;
    }

    public async Task Logout()
    {
        _serviceLogout.Logout(_userState.IdSession);
    } 
}