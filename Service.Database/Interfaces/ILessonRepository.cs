using MaClasse.Shared.Models.Lesson;

namespace Service.Database.Interfaces;

public interface ILessonRepository
{
    Task<Lesson> AddLesson(Lesson lesson, string idUser);
    Task<LessonBook> GetLessonBook(string userId);
}