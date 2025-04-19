using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.DashboardContent.Calendar;

public partial class DayView : ComponentBase
{
    [Parameter] public List<Calendar.CalendarEvent> Events { get; set; } = new();
    [Parameter] public DateTime? SelectedDate { get; set; }

    private IEnumerable<int> Hours => Enumerable.Range(8, 10);
}