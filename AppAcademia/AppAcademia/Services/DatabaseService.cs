using AppAcademia.Models;
using SQLite;

namespace AppAcademia.Services;

public sealed class DatabaseService
{
  private readonly SQLiteAsyncConnection _conn;
  public SQLiteAsyncConnection Connection => _conn;

  public DatabaseService(string dbPath)
  {
    _conn = new SQLiteAsyncConnection(
            new SQLiteConnectionString(dbPath, storeDateTimeAsTicks: false));
  }

  public async Task InitAsync()
  {

    await _conn.CreateTableAsync<Exercise>();

    var createTriggerSql = @"
CREATE TRIGGER IF NOT EXISTS trg_exercises_updated_at
AFTER UPDATE ON exercises
BEGIN
  UPDATE exercises
    SET UpdatedAt = CURRENT_TIMESTAMP
  WHERE rowid = NEW.rowid;
END;";

    await _conn.ExecuteAsync(createTriggerSql);
  }

  public Task<List<Exercise>> GetExercisesAsync()
  {
    return _conn.Table<Exercise>().ToListAsync();
  }

  public Task<List<Exercise>> GetExercisesByDateAsync(DateTime date)
  {
    var start = date.Date;
    var end = start.AddDays(1);
    // Query exercises where CreatedAt is within [start, end)
    return _conn.Table<Exercise>()
      .Where(e => e.CreatedAt >= start && e.CreatedAt < end)
      .OrderByDescending(e => e.CreatedAt)
      .ToListAsync();
  }

  public Task<int> SaveExerciseAsync(Exercise exercise)
  {
    if (exercise.Id != 0)
    {
      return _conn.UpdateAsync(exercise);
    }
    else
    {
      return _conn.InsertAsync(exercise);
    }
  }

  public Task<int> DeleteExerciseAsync(Exercise exercise)
  {
    return _conn.DeleteAsync(exercise);
  }
}
