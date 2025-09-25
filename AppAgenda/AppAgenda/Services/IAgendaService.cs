using AppAgenda.Models;

namespace AppAgenda.Services
{
  public interface IAgendaService
  {
    Task InicializarBancoDadosAsync();
    Task<List<Contato>> ObterTodosContatosAsync();
    Task<Contato?> ObterContatoPorIdAsync(int id);
    Task<int> AdicionarContatoAsync(Contato contato);
    Task<int> AtualizarContatoAsync(Contato contato);
    Task<int> ExcluirContatoAsync(Contato contato);
    Task<int> ExcluirContatoPorIdAsync(int id);
    Task<List<Contato>> BuscarContatosPorNomeAsync(string termo);
    Task<List<Contato>> BuscarContatosPorEmailAsync(string termo);
    Task<bool> EmailExisteAsync(string email, int? idParaIgnorar = null);
    Task<int> ContarContatosAsync();
  }
}