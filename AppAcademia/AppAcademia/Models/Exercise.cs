using SQLite;

namespace AppAcademia.Models;

/*
  1. Tipo de exercício (por exemplo, "Supino", "Agachamento",
"Flexão", etc.).
  2. Número de repetições.
  3. Carga utilizada (se aplicável).
  4. Foto do aparelho
*/

public class Exercise
{

  [PrimaryKey, AutoIncrement]
  public int Id { get; set; }

  [MaxLength(100)]
  public string ExerciseType { get; set; }
  public int Repetitions { get; set; }
  public double Load { get; set; }
  public string EquipmentPhotoPath { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.Now;

  public DateTime UpdatedAt { get; set; } = DateTime.Now;


  public Exercise()
  {
    ExerciseType = string.Empty;
    EquipmentPhotoPath = string.Empty;
  }

  public Exercise(string exerciseType, int repetitions, double load, string equipmentPhotoPath)
  {
    ExerciseType = exerciseType;
    Repetitions = repetitions;
    Load = load;
    EquipmentPhotoPath = equipmentPhotoPath;
  }
}
