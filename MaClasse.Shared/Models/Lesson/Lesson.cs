using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MaClasse.Shared.Models.Lesson;

public class Lesson
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string IdLesson { get; set; }
    
    [BsonElement("IdAppointment")]
    public string IdAppointment { get; set; }

    [BsonElement("Class")]
    public string? Class { get; set; }
    
    [BsonElement("Title")]
    public string? Tilte { get; set; }
    
    [BsonElement("Objective")]
    public string? Objective { get; set; }
    
    [BsonElement("Skills")]
    public string? Skills { get; set; }
    
    [BsonElement("Material")]
    public string? Material { get; set; }

    [BsonElement("Process")]
    public string? Process { get; set; }
    
    [BsonElement("Behavior")]
    public string? Behavior { get; set; }
    
    [BsonElement("TeacherTask\n")]
    public string? TeacherTask { get; set; }
    
    [BsonElement("StudentTask\n")]
    public string? StudentTask { get; set; }
    
    [BsonElement("SuccessCriteria\n")]
    public string? SuccessCriteria { get; set; }
    
    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; }
    
    
    [BsonElement("UpdatedAt")]
    public DateTime UpdatedAt { get; set; }
    
    [BsonElement("Documents")]
    public List<Documents> Documents { get; set; } = new List<Documents>();
    
}