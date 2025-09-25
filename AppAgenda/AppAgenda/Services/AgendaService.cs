using AppAgenda.Models;
using SQLite;

namespace AppAgenda.Services
{
    public class AgendaService : IAgendaService
    {
        private SQLiteAsyncConnection? _database;

        public async Task InicializarBancoDadosAsync()
        {
            if (_database is not null)
                return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "AppAgenda.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<Contato>();
        }

        private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
            if (_database is null)
                await InicializarBancoDadosAsync();
            
            return _database!;
        }

        public async Task<List<Contato>> ObterTodosContatosAsync()
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Contato>().OrderBy(c => c.Nome).ToListAsync();
        }

        public async Task<Contato?> ObterContatoPorIdAsync(int id)
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Contato>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> AdicionarContatoAsync(Contato contato)
        {
            var db = await GetDatabaseAsync();
            return await db.InsertAsync(contato);
        }

        public async Task<int> AtualizarContatoAsync(Contato contato)
        {
            var db = await GetDatabaseAsync();
            return await db.UpdateAsync(contato);
        }

        public async Task<int> ExcluirContatoAsync(Contato contato)
        {
            var db = await GetDatabaseAsync();
            return await db.DeleteAsync(contato);
        }

        public async Task<int> ExcluirContatoPorIdAsync(int id)
        {
            var db = await GetDatabaseAsync();
            return await db.DeleteAsync<Contato>(id);
        }

        public async Task<List<Contato>> BuscarContatosPorNomeAsync(string termo)
        {
            var db = await GetDatabaseAsync();
            
            if (string.IsNullOrWhiteSpace(termo))
                return await ObterTodosContatosAsync();

            return await db.Table<Contato>()
                          .Where(c => c.Nome.ToLower().Contains(termo.ToLower()))
                          .OrderBy(c => c.Nome)
                          .ToListAsync();
        }

        public async Task<List<Contato>> BuscarContatosPorEmailAsync(string termo)
        {
            var db = await GetDatabaseAsync();
            
            if (string.IsNullOrWhiteSpace(termo))
                return await ObterTodosContatosAsync();

            return await db.Table<Contato>()
                          .Where(c => c.Email.ToLower().Contains(termo.ToLower()))
                          .OrderBy(c => c.Nome)
                          .ToListAsync();
        }

        public async Task<bool> EmailExisteAsync(string email, int? idParaIgnorar = null)
        {
            var db = await GetDatabaseAsync();
            
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var query = db.Table<Contato>().Where(c => c.Email.ToLower() == email.ToLower());
            
            if (idParaIgnorar.HasValue)
            {
                query = query.Where(c => c.Id != idParaIgnorar.Value);
            }

            var contato = await query.FirstOrDefaultAsync();
            return contato is not null;
        }

        public async Task<int> ContarContatosAsync()
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Contato>().CountAsync();
        }
    }
}