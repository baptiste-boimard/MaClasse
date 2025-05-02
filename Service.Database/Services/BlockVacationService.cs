using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Database;
using MongoDB.Driver;
using Service.Database.Database;

namespace Service.Database.Services;

public class BlockVacationService
{
    private readonly MongoDbContext _mongoDbContext;

    public BlockVacationService(MongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<List<Appointment>> GetAppointmentWithoutVacation(
        string userId,
        Appointment appointment,
        CancellationToken ct = default)
    {
        //* Il faut trouver l'année d'arrêt de l'année scolaire
        // si mois entre spetembre et decembre alors StartDate.Year +1 sinon = startDate.year
        int terminatedYear = appointment.Start.Month >= 9 ? appointment.Start.Year + 1 : appointment.Start.Year;
        
        //* Recherche de la date des vacances d'été de cette année
        var scheduler = await _mongoDbContext.Schedulers
            .Find(Builders<Scheduler>.Filter.Eq(s => s.IdUser, userId))
            .FirstOrDefaultAsync(ct);

        var appointmentList = scheduler.Appointments;
        
        var vacationSummer = appointmentList
            .Select(list =>list)
            .FirstOrDefault(a => 
                a.Text.Contains("Vacances d'Été", StringComparison.OrdinalIgnoreCase) &&
                a.Start.Year == terminatedYear);

        var blockingList = appointmentList
            .Select(list => list)
            .Where(a => (a.Text.Contains("Vacance", StringComparison.OrdinalIgnoreCase)
                         || a.Text.Contains("Pont", StringComparison.OrdinalIgnoreCase)) &&
                        a.Start < vacationSummer.Start)
            .ToList();

        var appointmentListFinal = await GenerateWeeklyMondaysAsync(userId, appointment, vacationSummer, blockingList);

        return appointmentListFinal;
    }

    public async Task<List<Appointment>> GenerateWeeklyMondaysAsync(
        string userId,
        Appointment prototype,
        Appointment vacationSummer,
        List<Appointment> blockingList)
    {
        var results = new List<Appointment>();

        // Jour de la semaine cible (le jour de prototype.Start)
        var targetDow = prototype.Start.DayOfWeek;
        
        // 1) Trouve la première occurrence du même jour de semaine que prototype.Start
        var first = prototype.Start;
        int daysToAdd = ((int)targetDow - (int)first.DayOfWeek + 7) % 7;
        var occurrence = first.AddDays(daysToAdd);

        // 2) Tant qu’on est avant le début des vacances d’été
        while (occurrence < vacationSummer.Start)
        {
            // 3) Vérifie le blocage
            bool isBlocked = blockingList.Any(b =>
                occurrence.Date >= b.Start.Date &&
                occurrence.Date <  b.End.Date
            );

            if (!isBlocked)
            {
                // 4) Clone le prototype en ajustant date
                var appt = new Appointment
                {
                    Id           = Guid.NewGuid().ToString(),
                    Start        = occurrence,
                    End          = occurrence + (prototype.End - prototype.Start),
                    Text         = prototype.Text,
                    Color        = prototype.Color,
                    Recurring    = true,
                    IdRecurring  = prototype.IdRecurring
                };

                // 5) Enregistre et garde le résultat
                // await _schedulerRepository.AddAppointment(userId, appt);
                results.Add(appt);
            }

            // 6) Passe à la semaine suivante (7 jours)
            occurrence = occurrence.AddDays(7);
        }

        return results;
    }
}