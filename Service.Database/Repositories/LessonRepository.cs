using System.Text.Json;
using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Lesson;
using Microsoft.Extensions.Logging.Abstractions;
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

    public async Task<Lesson> GetLesson(string idAppointment, string idUser)
    {
        var existingLessonBook = await _mongoDbContext.LessonBooks
            .Find(l => l.IdUser == idUser)
            .FirstOrDefaultAsync();
        
        if (existingLessonBook == null) return null;

        var existingLesson = existingLessonBook.Lessons
            .FirstOrDefault(l => l.IdAppointment == idAppointment);

        if (existingLesson == null) return null;

        return existingLesson;
    }
    
    public async Task<Lesson> AddLesson(Lesson lesson, string idUser)
    {
        lesson.IdLesson = ObjectId.GenerateNewId().ToString();
        lesson.CreatedAt = DateTime.Now;
        lesson.UpdatedAt = DateTime.Now;

        var existingLessonBook = await _mongoDbContext.LessonBooks
            .Find(l => l.IdUser == idUser)
            .FirstOrDefaultAsync();

        if (existingLessonBook == null) return null;
        
        existingLessonBook.Lessons.Add(lesson);

        var result = await _mongoDbContext.LessonBooks
            .UpdateOneAsync(l => l.IdUser == idUser,
                Builders<LessonBook>.Update.Set(
                    l => l.Lessons, existingLessonBook.Lessons));

        if (result.ModifiedCount == 0) return null;
        
        return lesson;
    }

    public async Task<Lesson> UpdateLesson(Lesson lesson, string idUser)
    {
      
        // 🔍 Log complet de l'objet Lesson reçu
        var lessonLog = JsonSerializer.Serialize(lesson, new JsonSerializerOptions
        {
          WriteIndented = true,
          IgnoreNullValues = false // ou DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull avec .NET 6+
        });
        Console.WriteLine("📘 Contenu de l'objet Lesson reçu :\n" + lessonLog);
      
      
        var existingLessonBook = await _mongoDbContext.LessonBooks
            .Find(l => l.IdUser == idUser)
            .FirstOrDefaultAsync();

        if (existingLessonBook == null) return null;

        var existingLessonIndex = existingLessonBook.Lessons
            .FindIndex(l => l.IdLesson == lesson.IdLesson);

        if (existingLessonIndex == -1) return null;
        
        lesson.UpdatedAt = DateTime.Now;
        
        existingLessonBook.Lessons[existingLessonIndex] = lesson;
        
        try
        {
          var result = await _mongoDbContext.LessonBooks
                          .UpdateOneAsync(l => l.IdUser == idUser,
                              Builders<LessonBook>.Update.Set(
                                  l => l.Lessons, existingLessonBook.Lessons));
                        
            if (result.ModifiedCount == 0) return null;

            return lesson;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Lesson> DeleteLesson(Lesson lesson, string idUser)
    {
        var result = await _mongoDbContext.LessonBooks
            .UpdateOneAsync(
                Builders<LessonBook>.Filter.Eq(l => l.IdUser, idUser),
                Builders<LessonBook>.Update.PullFilter(l => l.Lessons,
                    Builders<Lesson>.Filter.Eq(l => l.IdLesson, lesson.IdLesson)));

        if (result.ModifiedCount == 0) return null;
        
        return lesson;
    }

    public async Task<Document> GetDocumentInLesson(RequestLesson request, string idUser)
    {
        var existingLessonBook = await _mongoDbContext.LessonBooks
            .Find(Builders<LessonBook>.Filter.Eq(
                l => l.IdUser, idUser))
            .FirstOrDefaultAsync();

        if (existingLessonBook == null) return null;

        var existingDocument = existingLessonBook.Lessons
            .FirstOrDefault(l => l.IdLesson == request.Lesson.IdLesson)
            .Documents.FirstOrDefault(d => d.IdDocument == request.Document.IdDocument);

        if (existingDocument == null) return null;
            
        return existingDocument;
    }

    public async Task<UpdateResult> DeleteDocumentInLesson(string idUser, string idLesson, string IdDocument)
    {
        var result = await _mongoDbContext.LessonBooks.UpdateOneAsync(
            Builders<LessonBook>.Filter.And(
                Builders<LessonBook>.Filter.Eq(lb => lb.IdUser, idUser),
                Builders<LessonBook>.Filter.ElemMatch(lb => lb.Lessons, l => l.IdLesson == idLesson)),
            Builders<LessonBook>.Update.PullFilter(
                "Lessons.$.Documents", Builders<Document>.Filter.Eq(
                    d => d.IdDocument, IdDocument)));

        if (result.ModifiedCount == 0) return null;
        
        return result;
    }

    public async Task<Document> GetDocument(string idDocument, string idUser)
    {
        Document doc = null;
        
        var userLessons = await _mongoDbContext.LessonBooks
            .Find(lb => lb.IdUser == idUser)
            .FirstOrDefaultAsync();

        foreach (var lesson in userLessons.Lessons)
        {
            doc = lesson.Documents
                .FirstOrDefault(d => d.IdDocument == idDocument);
            
            if (doc != null) continue;
        }
        
        if (doc != null) return doc;
        
        return null;
    }

    public async Task<UpdateResult> UpdateDocumentInLesson(string idUser, string idLesson, Document document)
    {
        var doc = await _mongoDbContext.LessonBooks.Find(lb => lb.IdUser == idUser).FirstOrDefaultAsync();
        
        var arrayFilters = new List<ArrayFilterDefinition>
        {
            // new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("lesson._id", new ObjectId(idLesson))),
            new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("lesson._id",new ObjectId(idLesson))),
            new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("doc._id",new ObjectId(document.IdDocument)))
        };
        
        var result = await _mongoDbContext.LessonBooks
            .UpdateOneAsync(
                Builders<LessonBook>.Filter.Eq(lb => lb.IdUser, idUser),
                Builders<LessonBook>.Update
                    .Set("Lessons.$[lesson].Documents.$[doc].Name", document.Name)
                    .Set("Lessons.$[lesson].Documents.$[doc].IdCloudinary", document.IdCloudinary),
                new UpdateOptions { ArrayFilters = arrayFilters });

        if (result.ModifiedCount == 0) return null;
        
        return result;
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

    public async Task<Dictionary<string, string>> GetLessonsByIdDocument(Document document, string idUser)
    {
        var result = new Dictionary<string, string>();
        
        //* Récupération du LessonBook de l'utilisateur
        var lessonBook = await _mongoDbContext.LessonBooks
            .Find(lb => lb.IdUser == idUser)
            .FirstOrDefaultAsync();
        
        if (lessonBook == null) return result;
        
        //* Recherche du document dans les Lessons
        foreach (var lesson in lessonBook.Lessons)
        {
            var matchingDocument = lesson.Documents
                .FirstOrDefault(d => d.IdDocument == document.IdDocument);

            if (matchingDocument != null)
            {
                result[lesson.IdLesson] = matchingDocument.IdDocument;
            }
        }
        
        return result;
    }
}

