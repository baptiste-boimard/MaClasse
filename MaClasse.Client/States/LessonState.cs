using System.Net;
using MaClasse.Shared.Models.Lesson;
using MaClasse.Shared.Models.Scheduler;

namespace MaClasse.Client.States;

public class LessonState
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly UserState _userState;

    public LessonState(
        HttpClient httpClient,
        IConfiguration configuration,
        UserState userState)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _userState = userState;
    }
    
    public event Action OnAppointmentSelected;
    public event Action OnChange;
    
    public Lesson Lesson { get; set; }
    public Appointment SelectedAppointment { get; set; }

    public async void SetLessonSelected(Appointment appointment)
    {
        //* Je récupére l'appointment selectionnée
        SelectedAppointment = appointment;
        
        //* Je vais voir en base de données si j'ai une Lesson liée
        var lesson = await GetLessonFromAppointment();

        Lesson = lesson;
        
        NotifyStateChanged();
    }

    public async Task<Lesson> GetLessonFromAppointment()
    {
        var lessonRequest = new LessonRequest
        {
            Appointment = SelectedAppointment,
            IdSession = _userState.IdSession
        };
        
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/get-lesson", lessonRequest);

        if (response.IsSuccessStatusCode)
        {
            var lesson = await response.Content.ReadFromJsonAsync<Lesson>();
            return lesson;
        }
        return new Lesson();
    }

    public async Task<bool> AddLesson(Lesson lesson, Appointment appointment)
    {
        lesson.IdAppointment = appointment.Id;

        var newRequestLesson = new RequestLesson
        {
            Lesson = lesson,
            IdSession = _userState.IdSession
        };
        
        var response = await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/database/add-lesson", newRequestLesson);

        if (response.IsSuccessStatusCode)
        {
            Lesson = await response.Content.ReadFromJsonAsync<Lesson>();
            return true;
        }

        return false;
    }

    public async void DeleteLesson(Lesson lesson)
    {
        var newRequestLesson = new RequestLesson
        {
            Lesson = lesson,
            IdSession = _userState.IdSession
        };
        
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/delete-lesson", newRequestLesson);

        if (response.IsSuccessStatusCode)
        {
            Lesson = new Lesson();
            NotifyStateChanged();
        }
    }
    
    public void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}