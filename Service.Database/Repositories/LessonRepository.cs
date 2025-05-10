using MaClasse.Shared.Models.Lesson;
using MongoDB.Bson;
using MongoDB.Driver;
using Service.Database.Database;
using Service.Database.Interfaces;

namespace Service.Database.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly MongoDbContext _mongoDbContext;

    public LessonRepository(MongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }
    
    public async Task<Lesson> AddLesson(Lesson lesson, string idUser)
    {
        return null;
    }

    public async Task<LessonBook> GetLessonBook(string userId)
    {
        return null;
    }

    public async Task<LessonBook> AddLessonBook(string userId)
    {
        var newLessonBook = new LessonBook
        {
            IdLessonBook = ObjectId.GenerateNewId().ToString(),
            IdUser = userId,
            Lessons = new List<Lesson>()
        };

        await _mongoDbContext.LessonBooks.InsertOneAsync(newLessonBook);
        
        return newLessonBook;
    }

    public async Task<LessonBook> DeleteLessonBook(string userId)
    {
        var deletedLessonBook = await _mongoDbContext.LessonBooks.FindOneAndDeleteAsync(
            Builders<LessonBook>.Filter.Eq(l => l.IdUser, userId));
        
        return deletedLessonBook;
    }
}