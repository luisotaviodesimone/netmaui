using System;
using SQLite;
using AppSqlite.Models;

namespace AppSqlite.Services;

public class DatabaseService
{
  private readonly SQLiteAsyncConnection _database;

  public DatabaseService(string dbPath)
  {
    _database = new SQLiteAsyncConnection(dbPath);
    _database.CreateTableAsync<Tarefa>().Wait();
  }

  public Task<List<Tarefa>> GetTarefasAsync() => _database.Table<Tarefa>().ToListAsync();

  public Task<int> SaveTarefaAsync(Tarefa tarefa) => _database.InsertAsync(tarefa);

  public Task<int> DeleteTarefaAsync(Tarefa tarefa) => _database.DeleteAsync(tarefa);
}
