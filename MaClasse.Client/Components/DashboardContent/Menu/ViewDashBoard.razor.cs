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
    private readonly LessonState _lessonState;

    public ViewDashBoard(
        UserState userState,
        ViewDashboardState viewDashboardState,
        SchedulerState schedulerState,
        HttpClient httpClient,
        IConfiguration configuration,
        LessonState lessonState)
    {
        _userState = userState;
        _viewDashboardState = viewDashboardState;
        _schedulerState = schedulerState;
        _httpClient = httpClient;
        _configuration = configuration;
        _lessonState = lessonState;
    }
    
    private List<UserDashboard> Dashboards;
    private static string _buttonTextOwner = "Vous";
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

        //* Récupération des dashboard rattachés
        _viewDashboardState.GetViewDashboardFromDatabase();

        }
    }

    private void SetButtonText(string userName, string userId)
    {
        _buttonText = userName;
        _lessonState.ResetLessonState();
        _schedulerState.SetViewDashboard(userId);
        _lessonState.SetViewDashboard(userId);
        _viewDashboardState.GetUserAppointments(userId);
        // _lessonState.SelectedAppointment = new Appointment();
        
    }
}