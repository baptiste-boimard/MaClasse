using MaClasse.Client.States;
using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Database;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace MaClasse.Client.Components.DashboardContent.Calendar;

public partial class AddAppointmentPage : ComponentBase
{
    private readonly HttpClient _httpClient;
    private readonly UserState _userState;
    private readonly SchedulerState _schedulerState;

    public AddAppointmentPage(
        HttpClient httpClient,
        UserState userState,
        SchedulerState schedulerState)
    {
        _httpClient = httpClient;
        _userState = userState;
        _schedulerState = schedulerState;
    }
    
    // private readonly DialogService _dialogService;
    //
    // public AddAppointmentPage(DialogService dialogService)
    // {
    //     _dialogService = dialogService;
    // }
    // [Parameter]
    // public DateTime Start { get; set; }
    //
    // [Parameter]
    // public DateTime End { get; set; }
    //
    // Appointment model = new Appointment();
    //
    // protected override void OnParametersSet()
    // {
    //     model.Start = Start;
    //     model.End = End;
    // }
    //
    // void OnSubmit(Appointment model)
    // {
    //     _dialogService.Close(model);
    // }
    
    [Parameter] public DateTime Start { get; set; }
    [Parameter] public DateTime End { get; set; }
    [Parameter] public bool IsEditMode { get; set; }

    [Parameter] public EventCallback<Appointment> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback<Appointment> OnDelete { get; set; }

    
    private DateTime? tempStartDate;
    private TimeSpan? tempStartTime;

    private DateTime? tempEndDate;
    private TimeSpan? tempEndTime;
    


    Appointment model = new Appointment();

    protected override void OnParametersSet()
    {
        tempStartDate = Start;
        tempStartTime = Start.TimeOfDay;

        tempEndDate = End;
        tempEndTime = End.TimeOfDay;
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
                
                //* Envoi requete Save vers le back
                var newSchedulerRequest = new SchedulerRequest
                {
                    IdSession = _userState.IdSession,
                    Appointment = new Appointment
                    {
                        Start = model.Start,
                        End = model.End,
                        Text = model.Text
                    }
                };
                
                //* Requete
                var response = await _httpClient.PostAsJsonAsync(
                    "https://localhost:7261/api/database/add-appointment", newSchedulerRequest);

                if (response.IsSuccessStatusCode)
                {
                    var appointmentList = await response.Content.ReadFromJsonAsync<List<Appointment>>();
                    
                    _schedulerState.SetAppointments(appointmentList);
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
            await OnDelete.InvokeAsync(model);
    }
}

