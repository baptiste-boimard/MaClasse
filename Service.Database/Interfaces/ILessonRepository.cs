using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Lesson;
using MongoDB.Driver;

namespace Service.Database.Interfaces;

public interface ILessonRepository
{
    Task<Lesson> GetLesson(string idAppointment, string idUser);
    Task<Lesson> AddLesson(Lesson lesson, string idUser);
    Task<Lesson> UpdateLesson(Lesson lesson, string idUser);
    Task<Lesson> DeleteLesson(Lesson lesson, string idUser);
    Task<Document> GetDocumentInLesson(RequestLesson request, string idUser);
    Task<UpdateResult> DeleteDocumentInLesson(string idUser, string idLesson, string IdDocument);
    Task<UpdateResult> UpdateDocumentInLesson(string idUser, string idLesson, Document document);
    Task<LessonBook> GetLessonBook(string userId);
    Task<LessonBook> DeleteLessonBook(string userId);
    Task<LessonBook> AddLessonBook(string userId);
    Task<Dictionary<string, string>> GetLessonsByIdDocument(Document document, string idUser);

}