using System.Collections.ObjectModel;
using AppAgenda.Models;
using AppAgenda.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppAgenda.MVVM.ViewModels;

/// <summary>
/// ViewModel para gerenciar a lista de contatos da agenda
/// </summary>
public partial class AgendaViewModel : ObservableObject
{
    private readonly IAgendaService _agendaService;

    [ObservableProperty]
    private ObservableCollection<Contato> _contatos;

    [ObservableProperty]
    private Contato? _contatoSelecionado;

    [ObservableProperty]
    private string _textoPesquisa = string.Empty;

    [ObservableProperty]
    private bool _carregando = false;

    [ObservableProperty]
    private string _mensagemStatus = string.Empty;

    [ObservableProperty]
    private bool _temContatos = false;

    public AgendaViewModel(IAgendaService agendaService)
    {
        _agendaService = agendaService;
        _contatos = new ObservableCollection<Contato>();
        
        // Inicializar comandos
        CarregarContatosCommand = new AsyncRelayCommand(CarregarContatosAsync);
        AdicionarContatoCommand = new AsyncRelayCommand(AdicionarContatoAsync);
        EditarContatoCommand = new AsyncRelayCommand<Contato>(EditarContatoAsync);
        ExcluirContatoCommand = new AsyncRelayCommand<Contato>(ExcluirContatoAsync);
        PesquisarCommand = new AsyncRelayCommand(PesquisarContatosAsync);
        LimparPesquisaCommand = new RelayCommand(LimparPesquisa);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);

        // Carregar contatos ao inicializar
        _ = Task.Run(async () => await CarregarContatosAsync());
    }

    #region Comandos

    public IAsyncRelayCommand CarregarContatosCommand { get; }
    public IAsyncRelayCommand AdicionarContatoCommand { get; }
    public IAsyncRelayCommand<Contato> EditarContatoCommand { get; }
    public IAsyncRelayCommand<Contato> ExcluirContatoCommand { get; }
    public IAsyncRelayCommand PesquisarCommand { get; }
    public IRelayCommand LimparPesquisaCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }

    #endregion

    #region Métodos dos Comandos

    /// <summary>
    /// Carrega todos os contatos do banco de dados
    /// </summary>
    private async Task CarregarContatosAsync()
    {
        try
        {
            Carregando = true;
            MensagemStatus = "Carregando contatos...";

            await _agendaService.InicializarBancoDadosAsync();
            var contatos = await _agendaService.ObterTodosContatosAsync();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Contatos.Clear();
                foreach (var contato in contatos)
                {
                    Contatos.Add(contato);
                }
            });

            AtualizarStatusContatos();
        }
        catch (Exception ex)
        {
            MensagemStatus = $"Erro ao carregar contatos: {ex.Message}";
            await Application.Current?.MainPage?.DisplayAlert("Erro", MensagemStatus, "OK");
        }
        finally
        {
            Carregando = false;
        }
    }

    /// <summary>
    /// Navega para a tela de adicionar novo contato
    /// </summary>
    private async Task AdicionarContatoAsync()
    {
        try
        {
            // Navegar para página de adicionar contato
            await Shell.Current.GoToAsync("//AdicionarContatoView");
        }
        catch (Exception ex)
        {
            await Application.Current?.MainPage?.DisplayAlert("Erro", $"Erro ao navegar: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Navega para a tela de editar contato existente
    /// </summary>
    private async Task EditarContatoAsync(Contato? contato)
    {
        if (contato == null) return;

        try
        {
            // Passar o ID do contato como parâmetro de navegação
            await Shell.Current.GoToAsync($"//EditarContatoView?id={contato.Id}");
        }
        catch (Exception ex)
        {
            await Application.Current?.MainPage?.DisplayAlert("Erro", $"Erro ao navegar: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Exclui um contato após confirmação do usuário
    /// </summary>
    private async Task ExcluirContatoAsync(Contato? contato)
    {
        if (contato == null) return;

        try
        {
            bool confirmar = await Application.Current?.MainPage?.DisplayAlert(
                "Confirmar Exclusão",
                $"Deseja realmente excluir o contato '{contato.Nome}'?",
                "Sim",
                "Não");

            if (confirmar)
            {
                Carregando = true;
                MensagemStatus = "Excluindo contato...";

                var resultado = await _agendaService.ExcluirContatoAsync(contato);

                if (resultado > 0)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Contatos.Remove(contato);
                    });

                    MensagemStatus = "Contato excluído com sucesso!";
                    AtualizarStatusContatos();

                    await Application.Current?.MainPage?.DisplayAlert("Sucesso", MensagemStatus, "OK");
                }
                else
                {
                    MensagemStatus = "Erro ao excluir contato.";
                    await Application.Current?.MainPage?.DisplayAlert("Erro", MensagemStatus, "OK");
                }
            }
        }
        catch (Exception ex)
        {
            MensagemStatus = $"Erro ao excluir contato: {ex.Message}";
            await Application.Current?.MainPage?.DisplayAlert("Erro", MensagemStatus, "OK");
        }
        finally
        {
            Carregando = false;
        }
    }

    /// <summary>
    /// Pesquisa contatos por nome ou email
    /// </summary>
    private async Task PesquisarContatosAsync()
    {
        try
        {
            Carregando = true;

            if (string.IsNullOrWhiteSpace(TextoPesquisa))
            {
                await CarregarContatosAsync();
                return;
            }

            MensagemStatus = "Pesquisando contatos...";

            var contatosPorNome = await _agendaService.BuscarContatosPorNomeAsync(TextoPesquisa);
            var contatosPorEmail = await _agendaService.BuscarContatosPorEmailAsync(TextoPesquisa);

            // Combinar resultados e remover duplicatas
            var contatosEncontrados = contatosPorNome
                .Union(contatosPorEmail, new ContatoComparer())
                .OrderBy(c => c.Nome)
                .ToList();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Contatos.Clear();
                foreach (var contato in contatosEncontrados)
                {
                    Contatos.Add(contato);
                }
            });

            MensagemStatus = $"Encontrados {contatosEncontrados.Count} contato(s)";
            AtualizarStatusContatos();
        }
        catch (Exception ex)
        {
            MensagemStatus = $"Erro na pesquisa: {ex.Message}";
            await Application.Current?.MainPage?.DisplayAlert("Erro", MensagemStatus, "OK");
        }
        finally
        {
            Carregando = false;
        }
    }

    /// <summary>
    /// Limpa o texto de pesquisa e recarrega todos os contatos
    /// </summary>
    private void LimparPesquisa()
    {
        TextoPesquisa = string.Empty;
        _ = Task.Run(async () => await CarregarContatosAsync());
    }

    /// <summary>
    /// Atualiza a lista de contatos (pull-to-refresh)
    /// </summary>
    private async Task RefreshAsync()
    {
        await CarregarContatosAsync();
    }

    #endregion

    #region Métodos Auxiliares

    /// <summary>
    /// Atualiza as propriedades relacionadas ao status dos contatos
    /// </summary>
    private void AtualizarStatusContatos()
    {
        TemContatos = Contatos.Any();
        
        if (!TemContatos && string.IsNullOrWhiteSpace(TextoPesquisa))
        {
            MensagemStatus = "Nenhum contato cadastrado. Toque em '+' para adicionar.";
        }
        else if (!TemContatos && !string.IsNullOrWhiteSpace(TextoPesquisa))
        {
            MensagemStatus = "Nenhum contato encontrado para a pesquisa.";
        }
        else if (string.IsNullOrWhiteSpace(TextoPesquisa))
        {
            MensagemStatus = $"{Contatos.Count} contato(s) cadastrado(s)";
        }
    }

    /// <summary>
    /// Método público para ser chamado quando um contato for adicionado/editado
    /// em outras páginas
    /// </summary>
    public async Task AtualizarListaAsync()
    {
        await CarregarContatosAsync();
    }

    #endregion
}

/// <summary>
/// Comparador para evitar contatos duplicados na pesquisa
/// </summary>
public class ContatoComparer : IEqualityComparer<Contato>
{
    public bool Equals(Contato? x, Contato? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        return x.Id == y.Id;
    }

    public int GetHashCode(Contato obj)
    {
        return obj.Id.GetHashCode();
    }
}