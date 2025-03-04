using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.DashboardContent;

public partial class DashboardContent : ComponentBase
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;

    public DashboardContent(
        HttpClient httpClient,
        NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
    }

    private string _token;
    
    protected override async Task OnInitializedAsync()
    {
        // Récupérer l'URL complète
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        // Utiliser QueryHelpers pour parser la query string
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
    
        if (query.TryGetValue("token", out var tokenValues))
        {
            var token = tokenValues.FirstOrDefault();
            if (!string.IsNullOrEmpty(token))
            {
                _token = token;
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}