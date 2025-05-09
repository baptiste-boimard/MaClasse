using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MaClasse.Shared.Models.Lesson;

public class LessonBook
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string IdLessonBook { get; set; }
    
    [BsonElement("IdUser")]
    public string IdUser { get; set; }
    
    [BsonElement("Lessons")]
    public List<Lesson> Lessons { get; set; }
    
}