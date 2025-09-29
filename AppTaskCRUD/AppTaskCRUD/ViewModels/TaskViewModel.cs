using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AppTaskCRUD.Models;
using AppTaskCRUD.Services;

namespace AppTaskCRUD.ViewModels;

public class TaskViewModel : INotifyPropertyChanged
{
  private string _newTaskTitle = string.Empty;
  private readonly ITaskDatabase _database = new TaskDatabase();

  public ObservableCollection<TaskItem> Tasks { get; set; }

  public string NewTaskTitle
  {
    get => _newTaskTitle;
    set
    {
      _newTaskTitle = value;
      OnPropertyChanged();
      ((Command)AddTaskCommand).ChangeCanExecute();
    }
  }

  public ICommand AddTaskCommand { get; }
  public ICommand DeleteTaskCommand { get; }

  public TaskViewModel()
  {
    Tasks = new ObservableCollection<TaskItem>();
    AddTaskCommand = new Command(async () => await AddTask(), CanAddTask);
    DeleteTaskCommand = new Command<TaskItem>(async (task) => await DeleteTask(task));

    LoadTasks();
  }

  private async void LoadTasks()
  {
    try
    {
      var tasks = await _database.GetTasksAsync();
      Tasks.Clear();
      foreach (var task in tasks)
      {
        Tasks.Add(task);
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Erro ao carregar tarefas: {ex.Message}");
    }
  }

  private async Task AddTask()
  {
    if (!string.IsNullOrWhiteSpace(NewTaskTitle))
    {
      var newTask = new TaskItem
      {
        Title = NewTaskTitle,
        IsDone = false
      };

      try
      {
        await _database.SaveTaskAsync(newTask);

        Tasks.Add(newTask);
        NewTaskTitle = string.Empty;
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"Erro ao adicionar tarefa: {ex.Message}");
      }
    }
  }

  private bool CanAddTask()
  {
    return !string.IsNullOrWhiteSpace(NewTaskTitle);
  }

  private async Task DeleteTask(TaskItem task)
  {
    if (task != null)
    {
      try
      {
        System.Diagnostics.Debug.WriteLine($"Deletando tarefa ID: {task.Id}, Title: {task.Title}");
        await _database.DeleteTaskAsync(task);

        Tasks.Remove(task);
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"Erro ao deletar tarefa: {ex.Message}");
      }
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}
