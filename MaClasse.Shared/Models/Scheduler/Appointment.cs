namespace MaClasse.Shared.Models;

public class Appointment
{
    public string? Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Text { get; set; }
    public string Color { get; set; }
    public bool Recurring { get; set; }
    public string? IdRecurring { get; set; }
}