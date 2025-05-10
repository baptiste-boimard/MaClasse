using MaClasse.Shared.Models.Lesson;

namespace Service.Database.Interfaces;

public interface ILessonRepository
{
    Task<Lesson> AddLesson(Lesson lesson, string idUser);
    Task<LessonBook> GetLessonBook(string userId);
    Task<LessonBook> DeleteLessonBook(string userId);
    Task<LessonBook> AddLessonBook(string userId);
}