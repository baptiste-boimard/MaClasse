using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Scheduler;

namespace MaClasse.Shared.Models.Lesson;

public class RequestLesson
{
    public Lesson? Lesson { get; set; }
    public string? IdSession { get; set; }
    public string? IdAppointement { get; set; }
    public Document? Document { get; set; }
}