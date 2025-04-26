using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MaClasse.Shared.Models.Database;

public class Scheduler
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string IdScheduler { get; set; }

    [BsonElement("IdUser")]
    public string IdUser { get; set; }

    [BsonElement("Appointments")]
    public List<Appointment> Appointments { get; set; } = new List<Appointment>();

    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("UpdatedAt")]
    public DateTime UpdatedAt { get; set; }
}