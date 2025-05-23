﻿using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaClasse.Client.Components.Auth;

public partial class ThermsDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

    public void CloseTherms()
    {
        MudDialog?.Close(DialogResult.Ok(true));
    }
}