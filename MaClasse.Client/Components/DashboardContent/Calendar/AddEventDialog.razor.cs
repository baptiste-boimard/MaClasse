using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Calendar;

public partial class AddEventDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    private DateTime? SelectedDate = DateTime.Today;
    private TimeSpan? StartTime = new(8, 0, 0);
    private TimeSpan? EndTime = new(9, 0, 0);

    Calendar.CalendarEvent NewEvent = new();

    void Cancel() => MudDialog.Cancel();

    void Save()
    {
        if (SelectedDate.HasValue && StartTime.HasValue && EndTime.HasValue)
        {
            NewEvent.Start = SelectedDate.Value.Date + StartTime.Value;
            NewEvent.End = SelectedDate.Value.Date + EndTime.Value;

            MudDialog.Close(DialogResult.Ok(NewEvent));
        }
    }
}