using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.Utils;

public partial class Hour : ComponentBase
{
    private string heure = "";

    private System.Timers.Timer? timer;

    protected override void OnInitialized()
    {
        MettreAJourHeure();

        timer = new System.Timers.Timer(1000); // mise à jour chaque seconde
        timer.Elapsed += (sender, args) =>
        {
            MettreAJourHeure();
            InvokeAsync(StateHasChanged);
        };
        timer.Start();
    }

    private void MettreAJourHeure()
    {
        var fuseauHoraire = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
        var maintenant = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, fuseauHoraire);
        heure = maintenant.ToString("HH:mm");
    }

    public void Dispose()
    {
        timer?.Stop();
        timer?.Dispose();
    }
}