using System.Collections.Generic;
using AppTaskCRUD.Models;

namespace AppTaskCRUD.Services;

public interface ITaskDatabase
{
  Task<List<TaskItem>> GetTasksAsync();
  Task<TaskItem> GetTaskAsync(int id);
  Task<int> SaveTaskAsync(TaskItem task);
  Task<int> DeleteTaskAsync(TaskItem task);
}
