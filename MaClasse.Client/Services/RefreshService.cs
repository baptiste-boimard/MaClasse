using MaClasse.Client.States;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MaClasse.Client.Services;

public class RefreshService
{
    private readonly HttpClient _httpClient;
    private readonly UserState _userState;
    private readonly NavigationManager _navigationManager;
    private readonly ServiceAuthentication _serviceAuthentication;
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;

    public RefreshService(
        HttpClient httpClient,
        UserState userState,
        NavigationManager navigationManager,
        ServiceAuthentication serviceAuthentication,
        IConfiguration configuration,
        UserService userService)
    {
        _httpClient = httpClient;
        _userState = userState;
        _navigationManager = navigationManager;
        _serviceAuthentication = serviceAuthentication;
        _configuration = configuration;
        _userService = userService;
    }

    public async Task RefreshLogin(string? resultValue)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/auth/refresh-user",
            new { Token = resultValue });

        if (response.IsSuccessStatusCode)
        {
            var returnResponse = await response.Content.ReadFromJsonAsync<AuthReturn>();
            _userService.AuthenticateUser(returnResponse.User);
            
            var newUserState = new UserState
            {
                IdSession = returnResponse.IdSession,
                Id = returnResponse.User.Id,
                Email = returnResponse.User.Email,
                Name = returnResponse.User.Name,
                Role = returnResponse.User.Role,
                Zone = returnResponse.User.Zone,
                GivenName = returnResponse.User.GivenName,
                FamilyName = returnResponse.User.FamilyName,
                Picture = returnResponse.User.Picture,
                CreatedAt = returnResponse.User.CreatedAt,
                UpdatedAt = returnResponse.User.UpdatedAt
            };
                
            _userState.SetUser(newUserState);
            
            _navigationManager.NavigateTo("/dashboard");
        }
    }
}