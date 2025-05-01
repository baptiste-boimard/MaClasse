using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using Service.Database.Database;
using Service.Database.Interfaces;

namespace Service.Database.Repositories;

public class SchedulerRepository : ISchedulerRepository
{
    private readonly MongoDbContext _mongoDbContext;

    public SchedulerRepository(
        MongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }
    
    public async Task<Scheduler> GetScheduler(string userId)
    {
        var existingScheduler = await _mongoDbContext.Schedulers
            .Find(Builders<Scheduler>.Filter.Eq(
                x => x.IdUser, userId))
            .FirstOrDefaultAsync();

        if (existingScheduler != null) return existingScheduler;

        return null;
    }
    
    public async Task<Scheduler> AddScheduler(string userId)
    {
        var newScheduler = new Scheduler
        {
            IdScheduler = ObjectId.GenerateNewId().ToString(),
            IdUser = userId,
            Appointments = new List<Appointment>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await _mongoDbContext.Schedulers.InsertOneAsync(newScheduler);
        
        return newScheduler;
    }
    
    public async Task<Appointment> GetOneAppointment(string userId, Appointment appointment)
    {
        var existingScheduler =  await _mongoDbContext.Schedulers
            .Find(s => s.IdUser == userId)
            .FirstOrDefaultAsync();
        
        if (existingScheduler == null) return null;
        
        var existingAppointment = existingScheduler.Appointments
            .FirstOrDefault(a => a.Start == appointment.Start && a.End == appointment.End);

        if (existingAppointment == null) return null;

        return existingAppointment;
    }

    public async Task<Appointment> GetOneAppointmentById(string userId, Appointment appointment)
    {
        var existingScheduler =  await _mongoDbContext.Schedulers
            .Find(s => s.IdUser == userId)
            .FirstOrDefaultAsync();
        
        if (existingScheduler == null) return null;
        
        var existingAppointment = existingScheduler.Appointments
            .FirstOrDefault(a => a.Id == appointment.Id);

        if (existingAppointment == null) return null;

        return existingAppointment;
    }
    
    public async Task<List<Appointment>> AddAppointment(string userId, Appointment appointment)
    {
        var updatedScheduler = await _mongoDbContext.Schedulers
            .FindOneAndUpdateAsync(
                Builders<Scheduler>.Filter.Eq(s => s.IdUser, userId),
                Builders<Scheduler>.Update.Push(s => s.Appointments, appointment),
                new FindOneAndUpdateOptions<Scheduler>
                {
                    ReturnDocument = ReturnDocument.After
                });
        
        if (updatedScheduler != null) return updatedScheduler.Appointments;
        
        return null;
    }

    public async Task<List<Appointment>> DeleteAppointment (string userId, Appointment appointment)
    {
        var deletedScheduler = await _mongoDbContext.Schedulers
            .FindOneAndUpdateAsync(
                Builders<Scheduler>.Filter.Eq(s => s.IdUser, userId),
                Builders<Scheduler>.Update.PullFilter(
                    s => s.Appointments,
                    a => a.Id == appointment.Id),
                new FindOneAndUpdateOptions<Scheduler>
                {
                    ReturnDocument = ReturnDocument.After
                });
        
        if (deletedScheduler != null) return deletedScheduler.Appointments;
        
        return null;
    }
}