using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Lesson;
using MaClasse.Shared.Models.Scheduler;

namespace Service.OAuth.Service.Interface;

public interface ICreateDataService
{
  Task<Scheduler> CreateDataScheduler(string userId);
  Task<Scheduler> GetDataScheduler(string userId);
  Task<Scheduler> AddHolidayToScheduler(UserProfile user);
  Task<LessonBook> CreateDateLessonBook(string userId);

}