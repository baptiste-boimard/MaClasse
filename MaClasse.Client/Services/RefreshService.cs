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
            _userService.AuthenticateUser(returnResponse.UserWithRattachment.UserProfile);
            
            var newUserState = new UserState
            {
                IdSession = returnResponse.IdSession,
                Id = returnResponse.UserWithRattachment.UserProfile.Id,
                IdRole = returnResponse.UserWithRattachment.UserProfile.IdRole,
                Email = returnResponse.UserWithRattachment.UserProfile.Email,
                Name = returnResponse.UserWithRattachment.UserProfile.Name,
                Role = returnResponse.UserWithRattachment.UserProfile.Role,
                Zone = returnResponse.UserWithRattachment.UserProfile.Zone,
                GivenName = returnResponse.UserWithRattachment.UserProfile.GivenName,
                FamilyName = returnResponse.UserWithRattachment.UserProfile.FamilyName,
                Picture = returnResponse.UserWithRattachment.UserProfile.Picture,
                CreatedAt = returnResponse.UserWithRattachment.UserProfile.CreatedAt,
                UpdatedAt = returnResponse.UserWithRattachment.UserProfile.UpdatedAt,
                AsDirecteur = returnResponse.UserWithRattachment.AsDirecteur,
                AsProfesseur = returnResponse.UserWithRattachment.AsProfesseur
            };
                
            _userState.SetUser(newUserState);
            
            _navigationManager.NavigateTo("/dashboard");
        }
    }
}