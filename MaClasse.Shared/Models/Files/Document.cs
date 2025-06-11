using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MaClasse.Shared.Models.Files;

public class Document
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string IdDocument { get; set; }
    
  [BsonElement("IdCloudinary")]
  public string? IdCloudinary { get; set; }

  [BsonElement("Name")]
  public string? Name { get; set; }
    
  [BsonElement("Url")]
  public string? Url { get; set; }
    
  [BsonElement("ThumbnailUrl")]
  public string? ThumbnailUrl { get; set; }
  
  [BsonElement("Format")]
  public string? Format { get; set; }
  
  [BsonElement("CreatedAt")]
  public DateTime CreatedAt { get; set; }
}