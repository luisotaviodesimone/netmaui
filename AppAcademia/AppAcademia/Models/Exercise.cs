using SQLite;
using System;

namespace AppAcademia.Models;

[Table("exercises")]
public class Exercise
{
  [PrimaryKey, AutoIncrement]
  public int Id { get; set; }

  [MaxLength(100), NotNull]
  public string ExerciseType { get; set; } = string.Empty;

  [NotNull]
  public int Repetitions { get; set; }

  [NotNull]
  public double Load { get; set; }

  public string EquipmentPhotoPath { get; set; } = string.Empty;

  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;

  public Exercise() { }

  public Exercise(string exerciseType, int repetitions, double load, string equipmentPhotoPath)
  {
    ExerciseType = exerciseType;
    Repetitions = repetitions;
    Load = load;
    EquipmentPhotoPath = equipmentPhotoPath;
  }
}
