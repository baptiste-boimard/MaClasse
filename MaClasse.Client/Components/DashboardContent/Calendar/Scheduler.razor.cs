using System.Globalization;
using MaClasse.Client.States;
using Radzen;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using Radzen.Blazor;

namespace MaClasse.Client.Components.DashboardContent.Calendar;

public partial class Scheduler : ComponentBase
{

    private readonly DialogService _dialogService;
    private readonly SchedulerState _schedulerState;


    public Scheduler(
        DialogService dialogService,
        SchedulerState schedulerState)
    {
        _dialogService = dialogService;
        _schedulerState = schedulerState;
    }

    private RadzenScheduler<Appointment> scheduler;
    private string schedulerWidth => selectedViewIndex == 0 ? "20rem" : "44rem";
    private TimeSpan startTime = new TimeSpan(6, 0, 0);
    private TimeSpan endTime = new TimeSpan(21, 30, 0);
    private bool datePickerOpen = false;
    private bool showAppointmentPanel = false;
    private DateTime _currentMonth = DateTime.Today;
    private DateTime currentDate = DateTime.Today;
    private int selectedViewIndex = 0;
    DateTime selectedStart;
    DateTime selectedEnd;
    private bool isEditMode = false;
    Dictionary<DateTime, string> events = new Dictionary<DateTime, string>();
    private IList<Appointment> appointments = new List<Appointment>();
    private Appointment selectedAppointment;

    
    protected override async Task OnInitializedAsync()
    {
        _schedulerState.OnChange += RefreshAppointments;
    
        //* Récupération des appointments avec l'heure local
        appointments = _schedulerState.Appointments
            .Select(a => new Appointment
            {
                Id = a.Id,
                Start = a.Start.ToLocalTime(),
                End = a.End.ToLocalTime(),
                Text = a.Text,
                Color = a.Color

            }).ToList();
    }
    
    private void RefreshAppointments()
    {
        appointments = _schedulerState.Appointments
            .Select(a => new Appointment
            {
                Id = a.Id,
                Start = a.Start.ToLocalTime(),
                End = a.End.ToLocalTime(),
                Text = a.Text,
                Color = a.Color

            }).ToList();
        InvokeAsync(() =>
        {
            // scheduler.Reload();
            StateHasChanged();

        });
    }
    
    void OnDaySelect(SchedulerDaySelectEventArgs args)
    {
        
    }
    
    void OnSlotRender(SchedulerSlotRenderEventArgs args)
    {
        if (args.View.Text == "Month" && args.Start.Date == DateTime.Today)
        {
            args.Attributes["style"] = "background: var(--rz-scheduler-highlight-background-color, rgba(255,220,40,.2));";
        }
    }
    
    async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
    {
        
        if (args.View.Text != "Year")
        {
            selectedStart = args.Start;
            selectedEnd = args.End;
            showAppointmentPanel = true;
            isEditMode = false;
        }
    }
    
    void OpenNewAppointment()
    {
        selectedStart = DateTime.Now;
        selectedEnd = DateTime.Now.AddHours(1);
        showAppointmentPanel = true;
    }
    
    void OnAppointmentSaved(Appointment appointment)
    {
        if (appointment.Id == null)
        {
            appointment.Id = Guid.NewGuid().ToString();
        }
        
        appointments.Add(appointment);
        showAppointmentPanel = false;
        isEditMode = false;
        scheduler.Reload();
        
        //! Enregistrement de l'appointement vers la base de données
    }
    
    async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<Appointment> args)
    {
        
        selectedStart = args.Data.Start;
        selectedEnd = args.Data.End;
        selectedAppointment = args.Data;
        isEditMode = true;
        showAppointmentPanel = true;
    }
    
    void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Appointment> args)
    {
        if (!string.IsNullOrEmpty(args.Data.Color))
        {
            // Applique la couleur de fond depuis l'objet
            args.Attributes["style"] = $"background-color: {args.Data.Color}; color: black;";
        }
    }

    async Task OnAppointmentMove(SchedulerAppointmentMoveEventArgs args)
    {
        var draggedAppointment = appointments.FirstOrDefault(
            x => x == args.Appointment.Data);

        if (draggedAppointment != null)
        {
            var duration = draggedAppointment.End - draggedAppointment.Start;

            if (args.SlotDate.TimeOfDay == TimeSpan.Zero)
            {
                draggedAppointment.Start = args.SlotDate.Date.Add(draggedAppointment.Start.TimeOfDay);
            }
            else
            {
                draggedAppointment.Start = args.SlotDate;
            }

            draggedAppointment.End = draggedAppointment.Start.Add(duration);

            await scheduler.Reload();
        }
    }
    
    private void OnDatePicked(DateTime? date)
    {
        if (date.HasValue)
        {
            currentDate = date.Value;
        }

        datePickerOpen = false;
    }
    
    void OnAppointmentDeleted(Appointment appointment)
    {
        var existing = appointments.FirstOrDefault(a => a.Id == appointment.Id);
        
        if (existing != null)
        {
            appointments.Remove(existing);
        }
        
        showAppointmentPanel = false;
        isEditMode = false;
        scheduler.Reload();
    }
    
    void ClosePanel()
    {
        showAppointmentPanel = false;
        isEditMode = false;
    }
    
    private void SetSchedulerView(int index)
    {
        selectedViewIndex = index;
        StateHasChanged(); 
    }
    
    private void GoToToday()
    {
        currentDate = DateTime.Today;
        StateHasChanged();
    }

    private void GoToPrevious()
    {
        currentDate = selectedViewIndex == 0 ? currentDate.AddDays(-1) : currentDate.AddDays(-7);
        StateHasChanged();
    }

    private void GoToNext()
    {
        currentDate = selectedViewIndex == 0 ? currentDate.AddDays(1) : currentDate.AddDays(7);
        StateHasChanged();
    }
    
    private string DisplayedDateLabel
    {
        get
        {
            if (selectedViewIndex == 0) 
            {
                return currentDate.ToString("dd/MM/yyyy");
            }
            else
            {
                var firstDayOfWeek = currentDate.StartOfWeek(DayOfWeek.Monday);
                var lastDayOfWeek = firstDayOfWeek.AddDays(6);
                return $"{firstDayOfWeek:dd/MM/yyyy} au {lastDayOfWeek:dd/MM/yyyy}";
            }
        }
    }
}