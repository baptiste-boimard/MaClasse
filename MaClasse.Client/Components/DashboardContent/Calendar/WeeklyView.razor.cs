using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.DashboardContent.Calendar;

public partial class WeeklyView : ComponentBase
{
    [Parameter] public List<Calendar.CalendarEvent> Events { get; set; } = new();
    [Parameter] public DateTime? SelectedDate { get; set; }

    private IEnumerable<DateTime> WeekDays =>
        Enumerable.Range(0, 7)
            .Select(i => StartOfWeek(SelectedDate!.Value).AddDays(i));

    private IEnumerable<int> Hours => Enumerable.Range(8, 11); // 8h à 18h

    private DateTime StartOfWeek(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    double HourHeightRem = 5.0;

    double GetTopOffset(DateTime start)
        => (start.Minute / 60.0) * HourHeightRem;

    double GetHeight(DateTime start, DateTime end)
        => (end - start).TotalMinutes / 60.0 * HourHeightRem;
}