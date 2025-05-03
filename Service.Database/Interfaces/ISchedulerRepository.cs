using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Scheduler;
using Service.Database.Database;

namespace Service.Database.Interfaces;

public interface ISchedulerRepository
{
    Task<Scheduler> GetScheduler(string userId);
    Task<Scheduler> AddScheduler(string userId);
    Task<Appointment> GetOneAppointment(string userId, Appointment appointment);
    Task<Appointment> GetOneAppointmentById(string userId, Appointment appointment);
    Task<List<Appointment>> AddAppointment(string userId, Appointment appointment);
    Task<Scheduler> AddListAppointment(string userId, List<Appointment> appointments);
    Task<List<Appointment>> UpdateAppointment(string userId, Appointment appointment);
    Task<List<Appointment>> DeleteAppointment(string userId, Appointment appointment);
    Task<List<Appointment>> DeleteListAppointment(string idUser, string idRecurring, DateTime startDate);
    Task<List<Appointment>> GetBlockVacation(string userId, Appointment appointment);
}