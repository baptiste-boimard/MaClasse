using MaClasse.Client.Components;
using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Scheduler;

namespace MaClasse.Client.States;

public class SchedulerState
{
    public event Action OnChange;
    
    public string IdScheduler { get; set; }
    public string IdUser { get; set; }
    public List<Appointment> Appointments { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string SchedulerDisplayed { get; set; }
    
    
    public void SetScheduler(SchedulerState schedulerState)
    {
        IdScheduler = schedulerState.IdScheduler;
        IdUser = schedulerState.IdUser;
        Appointments = schedulerState.Appointments;
        CreatedAt = schedulerState.CreatedAt;
        UpdatedAt = schedulerState.UpdatedAt;
        
        NotifyStateChanged();

    }
    
    public SchedulerState GetScheduler()
    {
        return this;
    }
    
    public void ResetSchedulerState()
    {
        IdScheduler = "";
        IdUser = "";
        Appointments = new List<Appointment>();
        CreatedAt = null;
        UpdatedAt = null;
        
        NotifyStateChanged();

    }
    
    public List<Appointment> SetAppointments(List<Appointment> appointments)
    {
        Appointments = appointments;
        NotifyStateChanged();
        return appointments;
    }

    public void SetViewDashboard(string userId)
    {
        SchedulerDisplayed = userId;
        NotifyStateChanged();
    }
    
    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}