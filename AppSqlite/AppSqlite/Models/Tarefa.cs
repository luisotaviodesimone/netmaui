using SQLite;

namespace AppSqlite.Models;

public class Tarefa
{
  [PrimaryKey, AutoIncrement]
  public int Id { get; set; }
  public string Nome { get; set; }
  public bool Concluida { get; set; }

  public override string ToString()
  {
    return $"{Nome} - {(Concluida ? "Concluída" : "Pendente")}";
  }
  
}
