using MaClasse.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MaClasse.Client.Components.Pages;

public partial class Home : ComponentBase
{
    private readonly RefreshService _refreshService;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private bool isCheckingAuth = true;
    
    public Home(
        RefreshService refreshService,
        ProtectedLocalStorage protectedLocalStorage)
    {
        _refreshService = refreshService;
        _protectedLocalStorage = protectedLocalStorage;
    }
    
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            //* Je regarde si j'ai quelque chose dans mon localstorage
            var result = await _protectedLocalStorage.GetAsync<string>("MaClasseAuth");

            if (result.Success)
            {
                await _refreshService.RefreshLogin(result.Value);
            }
            
            isCheckingAuth = false;
            StateHasChanged();
        }
    }
}