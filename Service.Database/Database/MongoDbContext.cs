using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Lesson;
using MaClasse.Shared.Models.Scheduler;
using MongoDB.Driver;

namespace Service.Database.Database;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    
    public MongoDbContext(IConfiguration configuration)
    {
        var mongoClient = new MongoClient(configuration.GetConnectionString("MongoDbContext"));
        _database = mongoClient.GetDatabase(configuration["ConnectionStrings:MongoDbSettings:DatabaseName"]);
    }
    
    public IMongoCollection<Scheduler> Schedulers => _database.GetCollection<Scheduler>("Schedulers");
    public IMongoCollection<LessonBook> LessonBooks => _database.GetCollection<LessonBook>("LessonBooks");

}