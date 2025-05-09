using MaClasse.Shared.Models.Scheduler;

namespace MaClasse.Shared.Models.Lesson;

public class RequestLesson
{
    public Lesson Lesson { get; set; }
    public string IdSession { get; set; }
}