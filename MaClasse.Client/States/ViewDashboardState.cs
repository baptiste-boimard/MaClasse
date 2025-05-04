using MaClasse.Client.Components.DashboardContent.Menu;
using MaClasse.Shared.Models.ViewDashboard;

namespace MaClasse.Client.States;

public class ViewDashboardState
{
    public ViewDashboardState()
    {
        
    }

    public event Action OnChange;
    
    public List<UserDashboard> DashBoards { get; set; }

    public void SetViewDashboard(ViewDashboardState viewDashboardState)
    {
        DashBoards = viewDashboardState.DashBoards;
        NotifyStateChanged();
    }

    public ViewDashboardState GetViewDashboard()
    {
        return this;
    }

    public void ResetViewDashboardState()
    {
        DashBoards = new List<UserDashboard>();
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}