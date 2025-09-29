using System;
using SQLite;

namespace AppTaskCRUD.Models;

public class TaskItem
{
  [PrimaryKey, AutoIncrement]
  public int Id { get; set; }

  [MaxLength(250)]
  public string Title { get; set; }

  [NotNull]
  public bool IsDone { get; set; } = false;
}
