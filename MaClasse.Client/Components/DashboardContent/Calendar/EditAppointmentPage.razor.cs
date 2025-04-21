using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace MaClasse.Client.Components.DashboardContent.Calendar;

public partial class EditAppointmentPage : ComponentBase
{
    private readonly DialogService _dialogService;

    public EditAppointmentPage(DialogService dialogService)
    {
        _dialogService = dialogService;
    }
    [Parameter]
    public Appointment Appointment { get; set; }

    Appointment model = new Appointment();

    protected override void OnParametersSet()
    {
        model = Appointment;
    }

    void OnSubmit(Appointment model)
    {
        _dialogService.Close(model);
    }
}