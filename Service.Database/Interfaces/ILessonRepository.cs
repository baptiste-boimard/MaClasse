using MaClasse.Shared.Models.Lesson;

namespace Service.Database.Interfaces;

public interface ILessonRepository
{
    Task<Lesson> GetLesson(string idAppointment, string idUser);
    Task<Lesson> AddLesson(Lesson lesson, string idUser);
    Task<Lesson> UpdateLesson(Lesson lesson, string idUser);
    Task<Lesson> DeleteLesson(Lesson lesson, string idUser);
    Task<LessonBook> GetLessonBook(string userId);
    Task<LessonBook> DeleteLessonBook(string userId);
    Task<LessonBook> AddLessonBook(string userId);
}