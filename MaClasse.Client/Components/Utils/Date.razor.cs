using Microsoft.AspNetCore.Components;

namespace MaClasse.Client.Components.Utils;

public partial class Date : ComponentBase
{
    private string dateDuJour = "";

    protected override void OnInitialized()
    {
        var culture = new System.Globalization.CultureInfo("fr-FR");
        var date = DateTime.Now;
        dateDuJour = date.ToString("dddd d MMMM yyyy", culture);
        // Met la première lettre en majuscule
        dateDuJour = char.ToUpper(dateDuJour[0]) + dateDuJour.Substring(1);
    }
}