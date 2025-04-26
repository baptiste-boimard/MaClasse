using System.Globalization;
using Radzen;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using Radzen.Blazor;

namespace MaClasse.Client.Components.DashboardContent.Calendar;

public partial class Scheduler : ComponentBase
{

    private readonly DialogService _dialogService;
    

    public Scheduler(DialogService dialogService)
    {
        _dialogService = dialogService;
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


    IList<Appointment> appointments = new List<Appointment>
    {
        // new Appointment { Start = DateTime.Today.AddDays(-2), End = DateTime.Today.AddDays(-2), Text = "Birthday" },
        // new Appointment { Start = DateTime.Today.AddDays(-11), End = DateTime.Today.AddDays(-10), Text = "Day off" },
        // new Appointment { Start = DateTime.Today.AddDays(-10), End = DateTime.Today.AddDays(-8), Text = "Work from home" },
        // new Appointment { Start = DateTime.Today.AddHours(10), End = DateTime.Today.AddHours(12), Text = "Online meeting" },
        // new Appointment { Start = DateTime.Today.AddHours(10), End = DateTime.Today.AddHours(13), Text = "Skype call" },
        // new Appointment { Start = DateTime.Today.AddHours(14), End = DateTime.Today.AddHours(14).AddMinutes(30), Text = "Dentist appointment" },
        // new Appointment { Start = DateTime.Today.AddDays(1), End = DateTime.Today.AddDays(12), Text = "Vacation" },
    };

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

        // if (args.View.Text != "Year")
        // {
        //     Appointment data = await _dialogService.OpenAsync<AddAppointmentPage>("Add Appointment",
        //         new Dictionary<string, object> { { "Start", args.Start }, { "End", args.End } });
        //
        //     if (data != null)
        //     {
        //         appointments.Add(data);
        //         await scheduler.Reload();
        //     }
        // }
        if (args.View.Text != "Year")
        {
            selectedStart = args.Start;
            selectedEnd = args.End;
            showAppointmentPanel = true;
            isEditMode = false; // <--- ici important

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
        appointments.Add(appointment);
        showAppointmentPanel = false;
        scheduler.Reload();
    }

    void ClosePanel()
    {
        showAppointmentPanel = false;
    }

    private Appointment selectedAppointment;
    
    async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<Appointment> args)
    {

        // var copy = new Appointment
        // {
        //     Start = args.Data.Start,
        //     End = args.Data.End,
        //     Text = args.Data.Text
        // };
        //
        // var data = await _dialogService.OpenAsync<EditAppointmentPage>("Edit Appointment", new Dictionary<string, object> { { "Appointment", copy } });
        //
        // if (data != null)
        // {
        //     args.Data.Start = data.Start;
        //     args.Data.End = data.End;
        //     args.Data.Text = data.Text;
        // }
        //
        // await scheduler.Reload();
        // On sélectionne les dates de l'événement existant
        selectedStart = args.Data.Start;
        selectedEnd = args.Data.End;

        // On met à jour le modèle si besoin (ex: texte de l'événement)
        selectedAppointment = args.Data;
        isEditMode = true;

        // On ouvre le panneau latéral (ou modal) pour édition
        showAppointmentPanel = true;
        
    }
    
    void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Appointment> args)
    {
        // args.Attributes["style"] = $"inset-inline-start: 5%; width: 90%;";

    }

    async Task OnAppointmentMove(SchedulerAppointmentMoveEventArgs args)
    {
        var draggedAppointment = appointments.FirstOrDefault(x => x == args.Appointment.Data);

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
    
    private void OnDatePicked(DateTime? date)
    {
        if (date.HasValue)
        {
            currentDate = date.Value;
        }

        datePickerOpen = false;
    }
}