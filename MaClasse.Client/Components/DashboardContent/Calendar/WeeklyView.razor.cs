using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.DashboardContent.Calendar;

public partial class WeeklyView : ComponentBase
{
    [Parameter] public List<Calendar.CalendarEvent> Events { get; set; } = new();
    [Parameter] public DateTime? SelectedDate { get; set; }

    private IEnumerable<DateTime> WeekDays =>Enumerable.Range(0, 7).Select(i => StartOfWeek(SelectedDate!.Value).AddDays(i));
    private IEnumerable<int> Hours => Enumerable.Range(8, 11);

    private DateTime StartOfWeek(DateTime date)
    {
        var diff = date.DayOfWeek - DayOfWeek.Monday;
        return date.AddDays(-diff);
    }
}