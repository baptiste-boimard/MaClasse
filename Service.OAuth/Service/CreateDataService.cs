using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Lesson;
using MaClasse.Shared.Models.Scheduler;
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
            $"{_configuration["Url:ApiGateway"]}/api/database/add-scheduler", new CreateDataRequest
            {
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
            $"{_configuration["Url:ApiGateway"]}/api/database/get-scheduler", new CreateDataRequest
            {
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

    public async Task<Scheduler> AddHolidayToScheduler(UserProfile user)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/add-holiday-appointment",
            user);

        if (response.IsSuccessStatusCode)
        {

            var scheduler = await response.Content.ReadFromJsonAsync<Scheduler>();
            return scheduler;
        }
        return new Scheduler();
    }

    public async Task<LessonBook> CreateDateLessonBook(string userId)
    {
        var newCreateDateRequest = new CreateDataRequest
        {
            UserId = userId
        };
        
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/add-lessonbook", newCreateDateRequest);

        if (response.IsSuccessStatusCode)
        {
            var newLessonBook = await response.Content.ReadFromJsonAsync<LessonBook>();

            if (newLessonBook != null) return newLessonBook;

        }
        
        return null;
    }

    // public async Task<LessonBook> GetDataLessonBook(string userId)
    // {
    //     var newCreateDateRequest = new CreateDataRequest
    //     {
    //         UserId = userId
    //     };
    //
    //     var response = await _httpClient.PostAsJsonAsync(
    //         $"{_configuration["Url:ApiGateway"]}/api/database/get-lessonbook", newCreateDateRequest);
    //
    //     if (response.IsSuccessStatusCode)
    //     {
    //         var lessonBook = await response.Content.ReadFromJsonAsync<LessonBook>();
    //
    //         if (lessonBook != null) return lessonBook;
    //     }
    //
    //     return null;
    // }
}
    
