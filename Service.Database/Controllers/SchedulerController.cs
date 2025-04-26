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

    public SchedulerController(
        UserService userService, ISchedulerRepository schedulerRepository)
    {
        _userService = userService;
        _schedulerRepository = schedulerRepository;
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
        };
        
        var addedAppointment = await _schedulerRepository.AddAppointment(userSession.UserId.ToString(), newAppointment);
        
        
        //* On va retourner la liste des Appointements et la stocker dans le useState
        return Ok(addedAppointment);
        // return Ok(appointment);
    }
    
    [HttpDelete]
    [Route("delete-appointment")]
    public async Task<IActionResult> DeleteAppointment()
    {
        return null;
    }

    [HttpPost]
    [Route("add-scheduler")]
    public async Task<IActionResult> AddScheduler([FromBody] CreateSchedulerRequest request)
    {
        var newScheduler = await _schedulerRepository.AddScheduler(request.UserId);
        
        return Ok(newScheduler);
    }
    
}