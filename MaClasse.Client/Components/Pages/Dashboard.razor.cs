using MaClasse.Client.States;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace MaClasse.Client.Components.Pages;

public partial class Dashboard : ComponentBase
{
    private readonly UserState _userState;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public Dashboard(
        UserState userState,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _userState = userState;
        _authenticationStateProvider = authenticationStateProvider;
    }
    
    private UserState? userInformation;

    protected override async Task OnInitializedAsync()
    {
        _userState.OnChange += HandleUserStateChanged;

        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is { IsAuthenticated: true })
        {
            userInformation = _userState.GetUser();
        }
    }

    private void HandleUserStateChanged()
    {
        userInformation = _userState.GetUser();
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _userState.OnChange -= HandleUserStateChanged;
    }
}