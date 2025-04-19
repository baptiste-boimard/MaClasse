using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Calendar;

public partial class Calendar : ComponentBase
{
    private readonly IDialogService _dialogService;

    public Calendar(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }
    
    private DateTime? SelectedDate = DateTime.Today;
    private bool IsWeekView { get; set; } = false;

    private List<CalendarEvent> Events = new();
    
    public bool AlarmOn { get; set; }


    private async Task OpenAddDialog()
    {
        var dialog = await _dialogService.ShowAsync<AddEventDialog>("Nouvel Événement");
        var result = await dialog.Result;

        if (!result.Canceled && result.Data is CalendarEvent newEvent)
        {
            Events.Add(newEvent);
        }
    }

    public class CalendarEvent
    {
        public string Title { get; set; } = string.Empty;
        public DateTime? Start { get; set; } = DateTime.Today;
        public DateTime? End { get; set; } = DateTime.Today.AddHours(1);
        public string Location { get; set; } = string.Empty;
    }
    
    private bool CalendarOpen { get; set; } = false;

    private void ToggleCalendarPopover()
    {
        CalendarOpen = !CalendarOpen;
    }
    
    private void GoToPrevious()
    {
        if (SelectedDate == null)
            SelectedDate = DateTime.Today;

        SelectedDate = IsWeekView
            ? SelectedDate.Value.AddDays(-7)
            : SelectedDate.Value.AddDays(-1);
    }

    private void GoToNext()
    {
        if (SelectedDate == null)
            SelectedDate = DateTime.Today;

        SelectedDate = IsWeekView
            ? SelectedDate.Value.AddDays(7)
            : SelectedDate.Value.AddDays(1);
    }
    
    private string GetDisplayDate()
    {
        if (SelectedDate == null)
            return "";

        var culture = new System.Globalization.CultureInfo("fr-FR");

        if (IsWeekView)
        {
            var startOfWeek = StartOfWeek(SelectedDate.Value);
            var endOfWeek = startOfWeek.AddDays(6);

            return $"Semaine du {startOfWeek:dd MMM} au {endOfWeek:dd MMM yyyy}";
        }

        return SelectedDate.Value.ToString("dddd dd MMMM yyyy", culture);
    }

    private DateTime StartOfWeek(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }
    
    private void OnDateSelected(DateTime? date)
    {
        SelectedDate = date;
        CalendarOpen = false;
    }
}