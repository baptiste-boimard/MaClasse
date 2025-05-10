using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Lesson;
using MaClasse.Shared.Models.Scheduler;
using Microsoft.AspNetCore.Mvc;
using Service.Database.Interfaces;
using Service.Database.Services;

namespace Service.Database.Controllers;

[ApiController]  
[Route("api")]   
public class LessonController : ControllerBase
{
    private readonly ILessonRepository _lessonRepository;
    private readonly UserService _userService;

    public LessonController(
        ILessonRepository lessonRepository,
        UserService userService)
    {
        _lessonRepository = lessonRepository;
        _userService = userService;
    }

    [HttpPost]
    [Route("get-lessonbook")]
    public async Task<IActionResult> GetLessonBook(CreateDataRequest request)
    {
        var existingLessonBook = _lessonRepository.GetLessonBook(request.UserId);

        if (existingLessonBook == null) return NotFound();

        return Ok(existingLessonBook);
    }

    [HttpPost]
    [Route("get-lesson")]
    public async Task<IActionResult> GetLessonByAppointmentId(Appointment appointment)
    {
        return Ok();
    }

    [HttpPost]
    [Route("add-lesson")]
    public async Task<IActionResult> AddLesson(RequestLesson request)
    {
        var idUser = _userService.GetUserByIdSession(request.IdSession).Result.UserId;
        
        var newLesson = _lessonRepository.AddLesson(request.Lesson, idUser);
        return Ok();
    }

    [HttpPost]
    [Route("add-lessonbook")]
    public async Task<IActionResult> AddLessonBook([FromBody] CreateDataRequest request)
    {
        var newLessonBook = await _lessonRepository.AddLessonBook(request.UserId);

        if (newLessonBook == null) return NotFound();

        return Ok(newLessonBook);
    }
        
        
    [HttpPost]
    [Route("delete-lessonbook")]
    public async Task<IActionResult> DeleteLessonBook([FromBody] DeleteUserRequest request)
    {
        var deletedLessonBook = await _lessonRepository.DeleteLessonBook(request.IdUser);

        if (deletedLessonBook == null) return NotFound();
        
        return Ok(deletedLessonBook);
    }
}