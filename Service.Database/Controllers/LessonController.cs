using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Files;
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
    public async Task<IActionResult> GetLessonByAppointmentId(LessonRequest request)
    {
        var idUser = _userService.GetUserByIdSession(request.IdSession).Result.UserId;

        var existingLesson = await _lessonRepository.GetLesson(
            request.Appointment.Id, idUser);

        if (existingLesson == null) return NotFound(null);
        
        return Ok(existingLesson);
    }

    [HttpPost]
    [Route("add-lesson")]
    public async Task<IActionResult> AddLesson(RequestLesson request)
    {
        var idUser =  _userService.GetUserByIdSession(request.IdSession).Result.UserId;

        //* Lesson ne possède pas d'Id donc c'est une création 
        if (request.Lesson.IdLesson == null)
        {
            var newLesson = await _lessonRepository.AddLesson(request.Lesson, idUser);
            
            if (newLesson == null) return BadRequest();
        
            return Ok(newLesson);
        }
        else
        { 
            var updatedLesson = await _lessonRepository.UpdateLesson(request.Lesson, idUser);

            if (updatedLesson == null) return BadRequest();
            
            return Ok(updatedLesson);
        }
        
    }


    [HttpPost]
    [Route("delete-lesson")]
    public async Task<IActionResult> DeleteLesson(RequestLesson request)
    {
        Lesson existingLesson;
        
        var idUser = _userService.GetUserByIdSession(request.IdSession).Result.UserId;

        if (request.IdAppointement != null)
        {
            existingLesson = await _lessonRepository.GetLesson(request.IdAppointement, idUser);
        }
        else
        {
            existingLesson = await _lessonRepository.GetLesson(request.Lesson.IdAppointment, idUser);
        }
        
        if (existingLesson == null) return NotFound();

        var deletedLesson = await _lessonRepository.DeleteLesson(existingLesson, idUser);

        if (deletedLesson == null) return NotFound();
        
        return Ok(deletedLesson);
    }


    [HttpPost]
    [Route("delete-document-in-lesson")]
    public async Task<IActionResult> DeleteDocumentInLesson([FromBody] RequestLesson request)
    {
        var idUser = _userService.GetUserByIdSession(request.IdSession).Result.UserId;

        var existingDocument = await _lessonRepository.GetDocumentInLesson(request, idUser);

        if (existingDocument == null) return NotFound();

        var resultDeletedDocument = await _lessonRepository.DeleteDocumentInLesson(
            idUser, request.Lesson.IdLesson, existingDocument.IdDocument);

        if (resultDeletedDocument == null) return NotFound();
        
        return Ok(request.Document);
    }
    
    [HttpPost]
    [Route("upload-document-in-lesson")]
    public async Task<IActionResult> UploadDocumentInLesson([FromBody] RequestLesson request)
    {
        var idUser = _userService.GetUserByIdSession(request.IdSession).Result.UserId;

        var existingDocument = await _lessonRepository.GetDocumentInLesson(request, idUser);

        if (existingDocument == null) return NotFound();

        var resultUpdatedDocument = await _lessonRepository.UpdateDocumentInLesson(
            idUser, request.Lesson.IdLesson, request.Document);

        if (resultUpdatedDocument == null) return NotFound();
        
        return Ok(request.Document);
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