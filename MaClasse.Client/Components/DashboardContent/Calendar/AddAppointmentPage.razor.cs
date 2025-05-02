using MaClasse.Client.States;
using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Database;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using Radzen;

namespace MaClasse.Client.Components.DashboardContent.Calendar;

public partial class AddAppointmentPage : ComponentBase
{
    private readonly HttpClient _httpClient;
    private readonly UserState _userState;
    private readonly SchedulerState _schedulerState;
    private readonly IConfiguration _configuration;

    public AddAppointmentPage(
        HttpClient httpClient,
        UserState userState,
        SchedulerState schedulerState,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _userState = userState;
        _schedulerState = schedulerState;
        _configuration = configuration;
    }
    
    
    [Parameter] public DateTime Start { get; set; }
    [Parameter] public DateTime End { get; set; }
    [Parameter] public bool IsEditMode { get; set; }
    [Parameter] public EventCallback<Appointment> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback<Appointment> OnDelete { get; set; }
    [Parameter] public Appointment Model { get; set; }
    
    private DateTime? tempStartDate;
    private TimeSpan? tempStartTime;
    private DateTime? tempEndDate;
    private TimeSpan? tempEndTime;
    private Appointment model = new Appointment();

    private string _colorValue;
    private bool _recurring;
    
    protected override void OnParametersSet()
    {
        tempStartDate = Start;
        tempStartTime = Start.TimeOfDay;

        tempEndDate = End;
        tempEndTime = End.TimeOfDay;
        
        if (IsEditMode && Model != null)
        {
            model = new Appointment
            {
                Id = Model.Id,
                Start = Model.Start,
                End = Model.End,
                Text = Model.Text,
                Color = Model.Color,
                Recurring = Model.Recurring
            };
            
            _colorValue = Model.Color;
            _recurring = Model.Recurring;
        }
        else
        {
            _recurring = false;
            _colorValue = "#8E7DFD"; 
        }
    }

    async Task Submit()
    {
        if (OnSave.HasDelegate)
        {
            if (
                tempStartDate.HasValue &&
                tempStartTime.HasValue &&
                tempEndDate.HasValue &&
                tempEndTime.HasValue &&
                model.Text != null
                )
            {
                model.Start = tempStartDate.Value.Date + tempStartTime.Value;
                model.End = tempEndDate.Value.Date + tempEndTime.Value;

                await OnSave.InvokeAsync(model);
                
                var newSchedulerRequest = new SchedulerRequest
                {
                    IdSession = _userState.IdSession,
                    Appointment = new Appointment
                    {
                        Id = model.Id,
                        Start = model.Start,
                        End = model.End,
                        Text = model.Text,
                        Color = _colorValue,
                        Recurring = _recurring
                    }
                };
                
                //* Requete si IsEditMode = false
                if (!IsEditMode)
                {
                    var response = await _httpClient.PostAsJsonAsync(
                        $"{_configuration["Url:ApiGateway"]}/api/database/add-appointment",
                        newSchedulerRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        var appointmentList = await response.Content.ReadFromJsonAsync<List<Appointment>>();
                    
                        _schedulerState.SetAppointments(appointmentList);
                    }
                }
                
                //* Requete si IsEditMode = true
                if (IsEditMode)
                {
                    var response = await _httpClient.PostAsJsonAsync(
                        $"{_configuration["Url:ApiGateway"]}/api/database/update-appointment",
                        newSchedulerRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        var appointmentList = await response.Content.ReadFromJsonAsync<List<Appointment>>();
                    
                        _schedulerState.SetAppointments(appointmentList);
                    }
                }
            }
        }
    }

    async Task Cancel()
    {
        if (OnCancel.HasDelegate)
            await OnCancel.InvokeAsync();
    }
    
    async Task Delete()
    {
        if (OnDelete.HasDelegate)
        {
            
            await OnDelete.InvokeAsync(model);
            
            //* Envoi requete Delete vers le back
            var newSchedulerRequest = new SchedulerRequest
            {
                IdSession = _userState.IdSession,
                Appointment = new Appointment
                {
                    Id = model.Id,
                    Start = model.Start,
                    End = model.End,
                    Text = model.Text,
                    Color = _colorValue,
                    Recurring = _recurring
                }
            };
            
            var response = await _httpClient.PostAsJsonAsync(
                $"{_configuration["Url:ApiGateway"]}/api/database/delete-appointment", newSchedulerRequest);

            if (response.IsSuccessStatusCode)
            {
                var appointmentList = await response.Content.ReadFromJsonAsync<List<Appointment>>();
                    
                _schedulerState.SetAppointments(appointmentList);
            }
        }
    }
}

