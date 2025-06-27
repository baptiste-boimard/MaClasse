namespace Service.OAuth.Service.Interface;

public interface IDeleteUserService
{
  Task DeleteLessonBook(string userId);
  Task DeleteScheduler(string userId);
}