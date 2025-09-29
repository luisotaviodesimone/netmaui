using System;
using SQLite;
using AppTaskCRUD.Models;

namespace AppTaskCRUD.Services;

public class TaskDatabase : ITaskDatabase
{
  private readonly SQLiteAsyncConnection _database;

  public TaskDatabase()
  {
    string path = Path.Combine(FileSystem.AppDataDirectory, "tasks.db");
    System.Diagnostics.Debug.WriteLine($"Database path: {path}");
    _database = new SQLiteAsyncConnection(path);
    _database.CreateTableAsync<TaskItem>().Wait();
  }

  public Task<List<TaskItem>> GetTasksAsync() => _database.Table<TaskItem>().ToListAsync();

  public Task<TaskItem> GetTaskAsync(int id) => _database.Table<TaskItem>().Where(t => t.Id == id).FirstOrDefaultAsync();

  public Task<int> SaveTaskAsync(TaskItem task)
  {
    if (task.Id != 0)
    {
      System.Diagnostics.Debug.WriteLine($"Atualizando tarefa ID: {task.Id}");
      return _database.UpdateAsync(task);
    }
    else
    {
      System.Diagnostics.Debug.WriteLine($"Inserindo nova tarefa Title: {task.Title}");
      return _database.InsertAsync(task);
    }
  }

  public Task<int> DeleteTaskAsync(TaskItem task) => _database.DeleteAsync(task);
}
