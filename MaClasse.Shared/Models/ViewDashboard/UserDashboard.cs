namespace MaClasse.Shared.Models.ViewDashboard;

public class UserDashboard
{
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public string UserName { get; set; }
    public Scheduler.Scheduler UserScheduler { get; set; }
}