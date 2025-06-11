using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Scheduler;
using MaClasse.Shared.Models.ViewDashboard;

namespace MaClasse.Client.States;

public class ViewDashboardState
{
    private readonly UserState _userState;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly SchedulerState _schedulerState;

    public ViewDashboardState(
        UserState userState,
        HttpClient httpClient,
        IConfiguration configuration,
        SchedulerState schedulerState)
    {
        _userState = userState;
        _httpClient = httpClient;
        _configuration = configuration;
        _schedulerState = schedulerState;
    }

    public event Action OnChange;
    
    public List<UserDashboard> DashBoards { get; set; }

    public async void GetViewDashboardFromDatabase()
    {
        List<UserProfile> usersProfile = new List<UserProfile>();
        List<Scheduler> schedulers = new List<Scheduler>();

        var viewDashboardRequest = new ViewDashboardRequest
        {
            IdSession = _userState.IdSession,
            AsDirecteur = _userState.AsDirecteur
        };
        
        //* A partir des rattachment on va chercher les infos des personnes
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/auth/get-rattachments-infos",
            viewDashboardRequest);

        if (response.IsSuccessStatusCode)
        {
            usersProfile = await response.Content.ReadFromJsonAsync<List<UserProfile>>();
        }
        
        var IdsProfesseur = usersProfile
            .Select(u => u.Id)
            .Distinct()
            .ToList();
        
        var responseScheduler = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/get-schedulers",
            IdsProfesseur);


        if (responseScheduler.IsSuccessStatusCode)
        {
            schedulers = await responseScheduler.Content.ReadFromJsonAsync<List<Scheduler>>();
        }
        
        //* J'ai les schedulers et les infos, je remplis mes States


        DashBoards = usersProfile
            .Select(user =>
            {
                var scheduler = schedulers.FirstOrDefault(
                    s => s.IdUser == user.Id);

                return new UserDashboard
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserName = user.Name,
                    UserScheduler = scheduler
                };
            }).ToList();
        
        SetViewDashboard(DashBoards);
    }

    public void SetViewDashboard(List<UserDashboard> userDashboards)
    {
        DashBoards = userDashboards;
        NotifyStateChanged();
    }

    public ViewDashboardState GetViewDashboard()
    {
        return this;
    }

    public void ResetViewDashboardState()
    {
        DashBoards = new List<UserDashboard>();
    }
    
    public async void GetUserAppointments(string userId)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/get-scheduler", new CreateDataRequest
            {
                UserId = userId
            });

        if (response.IsSuccessStatusCode)
        {
            var scheduler = await response.Content.ReadFromJsonAsync<Scheduler>();
            
            if (scheduler != null)
            {
                _schedulerState.SetAppointments(scheduler.Appointments);
                
                NotifyStateChanged();
            }
        }

    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}