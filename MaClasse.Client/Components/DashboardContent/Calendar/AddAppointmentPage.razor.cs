using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace MaClasse.Client.Components.DashboardContent.Calendar;

public partial class AddAppointmentPage : ComponentBase
{
    private readonly DialogService _dialogService;

    public AddAppointmentPage(DialogService dialogService)
    {
        _dialogService = dialogService;
    }
    [Parameter]
    public DateTime Start { get; set; }

    [Parameter]
    public DateTime End { get; set; }

    Appointment model = new Appointment();

    protected override void OnParametersSet()
    {
        model.Start = Start;
        model.End = End;
    }

    void OnSubmit(Appointment model)
    {
        _dialogService.Close(model);
    }
}