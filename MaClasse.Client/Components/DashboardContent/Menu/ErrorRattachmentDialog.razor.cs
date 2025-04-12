using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.DashboardContent.Menu;

public partial class ErrorRattachmentDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }
    
    [Parameter] public string Message { get; set; }
    [Parameter] public string Result { get; set; }
    
    private string CommonBoxStyle => "border-radius: 0.8rem; margin: 0.5rem; margin-top: 2rem;";
    
    private string GetBorderStyle()
    {
        var borderColor = Result switch
        {
            "Error" => "#FF4B5C",
            "Success" => "#1E88E5",
        };
    
        return $"border: 1px solid {borderColor}; {CommonBoxStyle}";
    }
    
    public void CloseErrorRattachment()
    {
        MudDialog?.Close(DialogResult.Ok(true));
    }
}

