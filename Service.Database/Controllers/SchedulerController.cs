using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Service.Database.Interfaces;
using Service.Database.Services;

namespace Service.Database.Controllers;

[ApiController]  
[Route("api")]   
public class SchedulerController :  ControllerBase
{
    private readonly UserService _userService;
    private readonly ISchedulerRepository _schedulerRepository;
    private readonly HolidaysService _holidaysService;

    public SchedulerController(
        UserService userService,
        ISchedulerRepository schedulerRepository,
        HolidaysService holidaysService)
    {
        _userService = userService;
        _schedulerRepository = schedulerRepository;
        _holidaysService = holidaysService;
    }
    
    [HttpPost]
    [Route("get-scheduler")]
    public async Task<IActionResult> GetScheduler(CreateSchedulerRequest request)
    {
        var scheduler = await _schedulerRepository.GetScheduler(request.UserId);
        
        if (scheduler == null)
        {
            return NotFound();
        }
        
        return Ok(scheduler);
    }

    [HttpPost]
    [Route("add-scheduler")]
    public async Task<IActionResult> AddScheduler([FromBody] CreateSchedulerRequest request)
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
        var existingAppointment = await _schedulerRepository.GetOneAppointment(userSession.UserId, request.Appointment);
        
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
            Color = request.Appointment.Color
        };
        
        var addedAppointment = await _schedulerRepository.AddAppointment(userSession.UserId.ToString(), newAppointment);
        
        
        //* On va retourner la liste des Appointements et la stocker dans le useState
        return Ok(addedAppointment);
        // return Ok(appointment);
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
        
        //* Si il existe on l'update
        
        var updatedAppointment = await _schedulerRepository.UpdateAppointment(
            userSession.UserId, request.Appointment);
        
        if (updatedAppointment == null) return BadRequest();

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

        //* Si il existe on le suprrime
        var deletedAppointment = await _schedulerRepository.DeleteAppointment(
            userSession.UserId, request.Appointment);
        
        if (deletedAppointment == null) return BadRequest();

        return Ok(deletedAppointment);

    }
    

    
}