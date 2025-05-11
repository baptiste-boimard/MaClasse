using MaClasse.Shared.Models.Scheduler;

namespace MaClasse.Shared.Models.Lesson;

public class LessonRequest
{
    public Appointment Appointment { get; set; }
    public string IdSession { get; set; }
}