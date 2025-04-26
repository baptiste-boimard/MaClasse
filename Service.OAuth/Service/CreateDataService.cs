using MaClasse.Shared.Models.Database;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Service.OAuth.Service;

public class CreateDataService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public CreateDataService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    public async Task<Scheduler> CreateDataScheduler(string userId)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/add-scheduler", new CreateSchedulerRequest{
               UserId = userId
            });

        if (response.IsSuccessStatusCode)
        {
            var newScheduler = await response.Content.ReadFromJsonAsync<Scheduler>();
            
            if (newScheduler != null)
            {
                return newScheduler;
            }
        }

        return null;
    }

    public async Task<Scheduler> GetDataScheduler(string userId)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/get-scheduler", new CreateSchedulerRequest{
                UserId = userId
            });

        if (response.IsSuccessStatusCode)
        {
            var newScheduler = await response.Content.ReadFromJsonAsync<Scheduler>();
            
            if (newScheduler != null)
            {
                return newScheduler;
            }
        }
        return null;
    }
}