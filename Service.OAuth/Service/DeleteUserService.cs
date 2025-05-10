using MaClasse.Shared.Models;

namespace Service.OAuth.Service;

public class DeleteUserService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public DeleteUserService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task DeleteLessonBook(string userId)
    {
        var newDeleteUserRequest = new DeleteUserRequest
        {
            IdUser = userId
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/delete-lessonbook", newDeleteUserRequest);

        if (response.IsSuccessStatusCode)
        {

        }
    }

    public async Task DeleteScheduler(string userId)
    {
        var newDeleteUserRequest = new DeleteUserRequest
        {
            IdUser = userId
        };

        var respones = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/delete-scheduler", newDeleteUserRequest);

        if (respones.IsSuccessStatusCode)
        {

        }
    }
}