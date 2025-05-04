using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Scheduler;
using MongoDB.Bson;
using MongoDB.Driver;
using Service.Database.Database;
using Service.Database.Interfaces;
using Service.Database.Services;

namespace Service.Database.Repositories;

public class SchedulerRepository : ISchedulerRepository
{
    private readonly MongoDbContext _mongoDbContext;
    private readonly BlockVacationService _blockVacationService;

    public SchedulerRepository(
        MongoDbContext mongoDbContext,
        BlockVacationService blockVacationService)
    {
        _mongoDbContext = mongoDbContext;
        _blockVacationService = blockVacationService;
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

    public async Task<List<Scheduler>> GetManyScheduler(List<string> idsProfesseur)
    {
        var schedulers = await _mongoDbContext.Schedulers
            .Find(
                Builders<Scheduler>.Filter.In(s => s.IdUser, idsProfesseur))
            .ToListAsync();

        if (schedulers.Count == 0) return null;

        return schedulers;
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

    public async Task<Scheduler> AddListAppointment(string userId, List<Appointment> appointments)
    {
        
        
        var updatedScheduler = await _mongoDbContext.Schedulers
            .FindOneAndUpdateAsync(
                Builders<Scheduler>.Filter.Eq(s => s.IdUser, userId),
                Builders<Scheduler>.Update.PushEach(s => s.Appointments, appointments),
                new FindOneAndUpdateOptions<Scheduler>
                {
                    ReturnDocument = ReturnDocument.After
                });

        return updatedScheduler;
    }

    public async Task<List<Appointment>> UpdateAppointment(string userId, Appointment appointment)
    {
        var updatedScheduler = await _mongoDbContext.Schedulers
            .FindOneAndUpdateAsync(
                Builders<Scheduler>.Filter.And(
             Builders<Scheduler>.Filter.Eq(
                        s => s.IdUser, userId),
                        Builders<Scheduler>.Filter.ElemMatch(
                        s => s.Appointments, a => a.Id == appointment.Id)),
                Builders<Scheduler>.Update
                    .Set("Appointments.$.Id", appointment.Id)
                    .Set("Appointments.$.Start", appointment.Start)
                    .Set("Appointments.$.End", appointment.End)
                    .Set("Appointments.$.Text", appointment.Text)
                    .Set("Appointments.$.Color", appointment.Color)
                    .Set("Appointments.$.Recurring", appointment.Recurring)
                    .Set("Appointments.$.IdRecurring", appointment.IdRecurring),
                    
                new FindOneAndUpdateOptions<Scheduler>
                {
                    ReturnDocument = ReturnDocument.After
                }
            );


        if (updatedScheduler == null) return null;

        return updatedScheduler.Appointments;

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

    public async Task<List<Appointment>> DeleteListAppointment(string idUser, string idRecurring, DateTime startDate)
    {
        //* Récupération des elements a delete
        var filterScheduler = Builders<Scheduler>.Filter.Eq(s => s.IdUser, idUser);

        var scheduler = await _mongoDbContext.Schedulers.Find(filterScheduler).FirstOrDefaultAsync();

        
        var deletedAppointments = scheduler.Appointments
            .Where(a => a.IdRecurring == idRecurring && a.Start > startDate)
            .ToList();

        await _mongoDbContext.Schedulers.UpdateOneAsync(
            filterScheduler,
            Builders<Scheduler>.Update.PullFilter(s => s.Appointments,
                a => a.IdRecurring == idRecurring && a.Start > startDate));

        return deletedAppointments;
    }

    public async Task<List<Appointment>> GetBlockVacation(string userId, Appointment appointment)
    {
        var appointmentList = await _blockVacationService.GetAppointmentWithoutVacation(userId, appointment);

        //* Une fois la liste des Appointmensts récupérés je l'ajoute à la BDD
        var newScheduler = await AddListAppointment(userId, appointmentList);

        if (newScheduler == null) return null;
        
        return newScheduler.Appointments;
        
    }
}