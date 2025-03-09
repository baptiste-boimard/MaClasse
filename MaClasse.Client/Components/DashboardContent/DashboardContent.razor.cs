using System.Net.Http.Headers;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.DashboardContent;

public partial class DashboardContent : ComponentBase
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly ServiceHashUrl _serviceHashUrl;

    public DashboardContent(
        HttpClient httpClient,
        NavigationManager navigationManager,
        ServiceHashUrl serviceHashUrl)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _serviceHashUrl = serviceHashUrl;
    }

    private string _token;
    
    protected override async Task OnInitializedAsync()
    {
        //* Récupérer l'URL complète
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        //* Utiliser QueryHelpers pour parser la query string
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
    
        if (query.TryGetValue("message", out var stringMessage))
        {
            var tokenMessage = stringMessage;
                    
            _token = _serviceHashUrl.DecryptErrorOAuth(tokenMessage) as string;
                
            //* Enregistrement du token dans l'HEAD du client http
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token.ToString());
        }    
    }
}