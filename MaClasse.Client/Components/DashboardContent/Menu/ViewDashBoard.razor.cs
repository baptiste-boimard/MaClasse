using MaClasse.Client.States;
using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Scheduler;
using MaClasse.Shared.Models.ViewDashboard;
using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.DashboardContent.Menu;

public partial class ViewDashBoard : ComponentBase, IAsyncDisposable
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

    private List<UserDashboard> Dashboards = [];
    private string _selectedUserId = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _selectedUserId = _userState.Id;
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
            _viewDashboardState.GetViewDashboardFromDatabase();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _viewDashboardState.OnChange -= RefreshViewDashboards;
    }

    private void OnDashboardChange(ChangeEventArgs e)
    {
        _selectedUserId = e.Value?.ToString() ?? _userState.Id;
        _lessonState.ResetLessonState();
        _schedulerState.SetViewDashboard(_selectedUserId);
        _lessonState.SetViewDashboard(_selectedUserId);
        _viewDashboardState.GetUserAppointments(_selectedUserId);
    }
}
