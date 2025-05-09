using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Service.Database.Interfaces;
using Service.Database.Services;
// ReSharper disable All

namespace Service.Database.Controllers;

[ApiController]  
[Route("api")]   
public class SchedulerController :  ControllerBase
{
    private readonly UserService _userService;
    private readonly ISchedulerRepository _schedulerRepository;
    private readonly HolidaysService _holidaysService;
    private readonly BlockVacationService _blockVacationService;

    public SchedulerController(
        UserService userService,
        ISchedulerRepository schedulerRepository,
        HolidaysService holidaysService,
        BlockVacationService blockVacationService)
    {
        _userService = userService;
        _schedulerRepository = schedulerRepository;
        _holidaysService = holidaysService;
        _blockVacationService = blockVacationService;
    }
    
    [HttpPost]
    [Route("get-scheduler")]
    public async Task<IActionResult> GetScheduler(CreateDataRequest request)
    {
        var scheduler = await _schedulerRepository.GetScheduler(request.UserId);
        
        if (scheduler == null)
        {
            return NotFound();
        }
        
        return Ok(scheduler);
    }

    [HttpPost]
    [Route("get-schedulers")]
    public async Task<IActionResult> GetManyScheduler(List<string> idsProfesseur)
    {
        var schedulers = await _schedulerRepository.GetManyScheduler(idsProfesseur);

        if (schedulers == null) return BadRequest();

        return Ok(schedulers);
    }
    
    
    [HttpPost]
    [Route("add-scheduler")]
    public async Task<IActionResult> AddScheduler([FromBody] CreateDataRequest request)
    {
        var newScheduler = await _schedulerRepository.AddScheduler(request.UserId);
        
        return Ok(newScheduler);
    }
    
    [HttpPost]
    [Route("add-appointment")]
    public async Task<IActionResult> AddAppointment([FromBody] SchedulerRequest request)
    {   
        /* je recupére un id Session et un appointment */
        //* Je cherche un user que je recupére avec mon userServices
        var userSession = await _userService.GetUserByIdSession(request.IdSession);
        
        
        //* verifie si l'appointment existe pour mon userId
        var existingAppointment = await _schedulerRepository.GetOneAppointment(
            userSession.UserId, request.Appointment);
        
        if (existingAppointment != null)
        {
            //! Message d'erreur à faire
            return BadRequest("L'appointment existe déjà");
        }
        
        //* Si il n'existe pas on le save
        var newAppointment = new Appointment
        {
            Id = Guid.NewGuid().ToString(),
            Start = request.Appointment.Start,
            End = request.Appointment.End,
            Text = request.Appointment.Text,
            Color = request.Appointment.Color,
            Recurring = request.Appointment.Recurring,
            IdRecurring = request.Appointment.Recurring 
                ? Guid.NewGuid().ToString() 
                : String.Empty
        };

        if (!newAppointment.Recurring)
        {
            var addedAppointment = await _schedulerRepository.AddAppointment(
                userSession.UserId.ToString(), newAppointment);
            
            
            //* On va retourner la liste des Appointements et la stocker dans le useState
            return Ok(addedAppointment);
        }
        else
        {
            //* Il faut préparer la liste pour qu'elle s'arrete à l'année scolaire en cours
            //* et ne pas apparaitre durant les vacances
            var newAppointments = await _schedulerRepository.GetBlockVacation(userSession.UserId, newAppointment);
            
            return Ok(newAppointments);
        }
    }


    [HttpPost]
    [Route("add-holiday-appointment")]
    public async Task<IActionResult> AddHolidayToScheduler([FromBody] UserProfile user)
    {
        //* Utilisation de HolidaysService pour aller récupérer les données de vacances auprès de l'api
        var holidaysAppointment = await _holidaysService.GetZoneBVacationsAsync(user);
        
        //* Save dans la BDD
        var newScheduler = await _schedulerRepository.AddListAppointment(user.Id, holidaysAppointment);

        if (newScheduler == null) return BadRequest();
        
        return Ok(newScheduler);
    }
    
    [HttpPost]
    [Route("update-appointment")]
    public async Task<IActionResult> UpdateAppointment([FromBody] SchedulerRequest request)
    {
        //* Je cherche un user que je recupére avec mon userServices
        var userSession = await _userService.GetUserByIdSession(request.IdSession);
        
        //* verifie si l'appointment existe pour mon userId
        var existingAppointment = await _schedulerRepository.GetOneAppointmentById(
            userSession.UserId, request.Appointment);
        
        if (existingAppointment == null) return BadRequest();
        
        
        
        //* Si c'est un recurring et qu'on update en garder le recurring
        if (request.Appointment.Recurring && !string.IsNullOrEmpty(request.Appointment.IdRecurring))
        {
            var idRecurring = request.Appointment.IdRecurring;
            var startDate = request.Appointment.Start;

            var appointmenetsDeleted =
                await _schedulerRepository.DeleteListAppointment(userSession.UserId, idRecurring, startDate);

            if (appointmenetsDeleted == null) return BadRequest();

            var newAppointments =
                await _schedulerRepository.GetBlockVacation(userSession.UserId, request.Appointment);
            
            if (newAppointments == null) return BadRequest();
            
            //! On Supprime le cas qui serait à update car il est recrée par GetAppointmentWithoutVacation
            var listAppointmentAfterDelete = 
                await _schedulerRepository.DeleteAppointment(userSession.UserId, request.Appointment);

            if (listAppointmentAfterDelete == null) return BadRequest();

            //Je renvoi la liste apres le delete
            return Ok(listAppointmentAfterDelete);
        }
        
        
        
        
        //* Cas ou l'on ajoute la récurrence
        if (request.Appointment.Recurring && string.IsNullOrEmpty(request.Appointment.IdRecurring))
        {
            // on met a jour l'appointement
            request.Appointment.IdRecurring = Guid.NewGuid().ToString();
            
            // il faut créer les autres appointement récurrent
            var newRecurringAppointment =
                await _blockVacationService.GetAppointmentWithoutVacation(userSession.UserId, request.Appointment);

            if (newRecurringAppointment == null) return null;
            
            // On ajoute ces cas dans la BDD
            var recurringAppointmentAdded =
                await _schedulerRepository.AddListAppointment(userSession.UserId, newRecurringAppointment);

            if (recurringAppointmentAdded == null) return null;
            
            //! On Supprime le cas qui vient d'etre update cat il est recrée par GetAppointmentWithoutVacation
            var listAppointmentAfterDelete = 
                await _schedulerRepository.DeleteAppointment(userSession.UserId, request.Appointment);

            if (listAppointmentAfterDelete == null) return BadRequest();

            //Je renvoi la liste apres le delete
            return Ok(listAppointmentAfterDelete);

        }
        
        
        
        
        //* Cas ou l'on supprime la récurrence
        if (!request.Appointment.Recurring && !string.IsNullOrEmpty(request.Appointment.IdRecurring))
        {
            var idRecurring = request.Appointment.IdRecurring;
            var startDate = request.Appointment.Start;
            
            //* Update l'appointement en request pour enlever la récurence
            request.Appointment.Recurring = false;
            request.Appointment.IdRecurring = String.Empty;
            
            var appointmenetsDeleted =
                await _schedulerRepository.DeleteListAppointment(userSession.UserId, idRecurring, startDate);

            if (appointmenetsDeleted == null) return BadRequest();
        }
        
        
        //* Dans tous les cas on update l'appointement et on renvoie la liste des appointments
        var updatedAppointment = await _schedulerRepository.UpdateAppointment(
            userSession.UserId, request.Appointment);
        
        // if (updatedAppointment == null) return BadRequest();
        
        return Ok(updatedAppointment);
    }
    
    [HttpPost]
    [Route("delete-appointment")]
    public async Task<IActionResult> DeleteAppointment([FromBody] SchedulerRequest request)
    {
        //* Je cherche un user que je recupére avec mon userServices
        var userSession = await _userService.GetUserByIdSession(request.IdSession);
        
        //* verifie si l'appointment existe pour mon userId
        var existingAppointment = await _schedulerRepository.GetOneAppointmentById(
            userSession.UserId, request.Appointment);

        if (existingAppointment == null) return BadRequest();

        //* Si il existe et pas de récurrence on le suprrime
        if (!request.Appointment.Recurring)
        {
            var deletedAppointment = await _schedulerRepository.DeleteAppointment(
                userSession.UserId, request.Appointment);
        
            if (deletedAppointment == null) return BadRequest();

            return Ok(deletedAppointment);
        }
        else
        {
            //* On supprime aussi celuide base
            var deletedAppointment =
                await _schedulerRepository.DeleteAppointment(userSession.UserId, request.Appointment);

            if (deletedAppointment == null) return BadRequest();
            
            //* Si récurrence on supprime toutes les récurrences a partir de la date
            var idRecurring = request.Appointment.IdRecurring;
            var startDate = request.Appointment.Start;

            var deletedAppointments =
                await _schedulerRepository.DeleteListAppointment(userSession.UserId, idRecurring, startDate);

            if (deletedAppointments.Count == 0) return BadRequest();
            
            
            
            //* je récupére la liste des appointmenst restant
            var scheduler = await _schedulerRepository.GetScheduler(userSession.UserId);

            if (scheduler == null) return BadRequest();

            return Ok(scheduler.Appointments);
        }
    }
}