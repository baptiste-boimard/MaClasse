using MaClasse.Client.States;
using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Scheduler;
using MaClasse.Shared.Models.ViewDashboard;
using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.DashboardContent.Menu;

public partial class ViewDashBoard : ComponentBase
{
    private readonly UserState _userState;
    private readonly ViewDashboardState _viewDashboardState;
    private readonly SchedulerState _schedulerState;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ViewDashBoard(
        UserState userState,
        ViewDashboardState viewDashboardState,
        SchedulerState schedulerState,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _userState = userState;
        _viewDashboardState = viewDashboardState;
        _schedulerState = schedulerState;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    private List<UserProfile> usersProfile;
    private List<Scheduler> Schedulers;
    private List<UserDashboard> Dashboards;
    private static string _buttonTextOwner = "Votre Dashboard";
    private string _buttonText = _buttonTextOwner;

    
    protected override async Task OnInitializedAsync()
    {
        _viewDashboardState.OnChange += RefreshViewDashboards;
    }
    
    private void RefreshViewDashboards()
    {
        Dashboards = _viewDashboardState.DashBoards
            .Select(d => new UserDashboard
            {
                UserId = d.UserId,
                UserEmail = d.UserEmail,
                UserName = d.UserName,
                UserScheduler = d.UserScheduler})
            .ToList();
        
        InvokeAsync(() => { StateHasChanged(); });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
             //* Au chargement de notre composant on va charger les infos concernant
        //* les rattachements

        var listProf = _userState.AsDirecteur;
        var viewDashboardRequest = new ViewDashboardRequest
        {
            IdSession = _userState.IdSession,
            AsDirecteur = _userState.AsDirecteur
        };
        
        // a partir des rattachment on va chercher les infos des personnes
        // pour les mettre dans ViewDashboardState.
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/auth/get-rattachments-infos",
            viewDashboardRequest);

        if (response.IsSuccessStatusCode)
        {
            usersProfile = await response.Content.ReadFromJsonAsync<List<UserProfile>>();
        }
        
        // j'ai les id des prof et leur infos je vais recupere le scheduler
        // extaction des idProf pour demander les scheduler

        var IdsProfesseur = usersProfile
            .Select(u => u.Id)
            .Distinct()
            .ToList();
        
        var responseScheduler = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Url:ApiGateway"]}/api/database/get-schedulers",
        IdsProfesseur);


        if (responseScheduler.IsSuccessStatusCode)
        {
            Schedulers = await responseScheduler.Content.ReadFromJsonAsync<List<Scheduler>>();
        }
        
        // j'ai les schedulers et les infos
        // je remplis mon state

        var viewDashboardState = new ViewDashboardState
        {
            DashBoards = usersProfile
                .Select(user =>
                {
                    var scheduler = Schedulers.FirstOrDefault(
                        s => s.IdUser == user.Id);

                    return new UserDashboard
                    {
                        UserId = user.Id,
                        UserEmail = user.Email,
                        UserName = user.Name,
                        UserScheduler = scheduler
                    };
                }).ToList()
        };
        
        _viewDashboardState.SetViewDashboard(viewDashboardState);
        
        }
    }

    private void SetButtonText(string userName, string userId)
    {
        _buttonText = userName;
        // action qui lance l'affichahe du dashboard
        // if (userName == "Votre Dashboard") userId = _userState.Id;
        _schedulerState.SetViewDashboard(userId);
    }
}