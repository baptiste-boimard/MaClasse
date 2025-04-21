using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace MaClasse.Client.Components.DashboardContent.Calendar;

public partial class AddAppointmentPage : ComponentBase
{
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

    [Parameter] public EventCallback<Appointment> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    
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
            if (tempStartDate.HasValue && tempStartTime.HasValue && tempEndDate.HasValue && tempEndTime.HasValue)
            {
                model.Start = tempStartDate.Value.Date + tempStartTime.Value;
                model.End = tempEndDate.Value.Date + tempEndTime.Value;

                await OnSave.InvokeAsync(model);
            }
        }
    }

    async Task Cancel()
    {
        if (OnCancel.HasDelegate)
            await OnCancel.InvokeAsync();
    }
}

