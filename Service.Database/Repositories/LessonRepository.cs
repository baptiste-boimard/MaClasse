using MaClasse.Shared.Models.Lesson;
using Service.Database.Interfaces;

namespace Service.Database.Repositories;

public class LessonRepository : ILessonRepository
{
    public async Task<Lesson> AddLesson(Lesson lesson, string idUser)
    {
        return null;
    }

    public async Task<LessonBook> GetLessonBook(string userId)
    {
        return null;
    }
}