using AppAgenda.Models;
using AppAgenda.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppAgenda.MVVM.ViewModels;

/// <summary>
/// ViewModel para adicionar ou editar contatos
/// </summary>
public partial class ContatoViewModel : BaseViewModel
{
    private readonly IAgendaService _agendaService;

    [ObservableProperty]
    private Contato _contato;

    [ObservableProperty]
    private bool _isEdicao = false;

    [ObservableProperty]
    private string _tituloPagina = "Novo Contato";

    [ObservableProperty]
    private string _textoBotao = "Salvar";

    public ContatoViewModel(IAgendaService agendaService)
    {
        _agendaService = agendaService;
        _contato = new Contato();

        // Inicializar comandos
        SalvarCommand = new AsyncRelayCommand(SalvarContatoAsync, CanSalvar);
        CancelarCommand = new AsyncRelayCommand(CancelarAsync);
        
        // Observar mudanças nas propriedades do contato para validação
        Contato.PropertyChanged += (s, e) => SalvarCommand.NotifyCanExecuteChanged();
    }

    #region Comandos

    public IAsyncRelayCommand SalvarCommand { get; }
    public IAsyncRelayCommand CancelarCommand { get; }

    #endregion

    #region Métodos dos Comandos

    /// <summary>
    /// Salva o contato no banco de dados
    /// </summary>
    private async Task SalvarContatoAsync()
    {
        await ExecutarSeguroAsync(async () =>
        {
            // Validar se o email já existe (se preenchido)
            if (!string.IsNullOrWhiteSpace(Contato.Email))
            {
                bool emailExiste = await _agendaService.EmailExisteAsync(Contato.Email, IsEdicao ? Contato.Id : null);
                if (emailExiste)
                {
                    await ExibirAlertaAsync("Erro", "Este email já está cadastrado para outro contato.");
                    return;
                }
            }

            int resultado;
            string mensagemSucesso;

            if (IsEdicao)
            {
                resultado = await _agendaService.AtualizarContatoAsync(Contato);
                mensagemSucesso = "Contato atualizado com sucesso!";
            }
            else
            {
                resultado = await _agendaService.AdicionarContatoAsync(Contato);
                mensagemSucesso = "Contato adicionado com sucesso!";
            }

            if (resultado > 0)
            {
                await ExibirAlertaAsync("Sucesso", mensagemSucesso);
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await ExibirAlertaAsync("Erro", "Não foi possível salvar o contato.");
            }
        }, "Erro ao salvar contato");
    }

    /// <summary>
    /// Cancela a operação e volta para a tela anterior
    /// </summary>
    private async Task CancelarAsync()
    {
        bool temAlteracoes = IsEdicao ? ContatoFoiModificado() : ContatoTemDados();

        if (temAlteracoes)
        {
            bool confirmar = await ExibirConfirmacaoAsync(
                "Descartar Alterações",
                "Existem alterações não salvas. Deseja descartar?",
                "Descartar",
                "Continuar Editando");

            if (!confirmar) return;
        }

        await Shell.Current.GoToAsync("..");
    }

    #endregion

    #region Métodos Auxiliares

    /// <summary>
    /// Valida se o contato pode ser salvo
    /// </summary>
    private bool CanSalvar()
    {
        return !IsBusy && 
               !string.IsNullOrWhiteSpace(Contato?.Nome) &&
               Contato.Nome.Length <= 100 &&
               (string.IsNullOrWhiteSpace(Contato.Email) || 
                (Contato.Email.Length <= 150 && Contato.IsValidEmail()));
    }

    /// <summary>
    /// Verifica se o contato foi modificado (para modo edição)
    /// </summary>
    private bool ContatoFoiModificado()
    {
        // Implementar comparação com valores originais se necessário
        return true; // Por enquanto, sempre considera modificado
    }

    /// <summary>
    /// Verifica se o contato tem dados preenchidos (para modo criação)
    /// </summary>
    private bool ContatoTemDados()
    {
        return !string.IsNullOrWhiteSpace(Contato?.Nome) ||
               !string.IsNullOrWhiteSpace(Contato?.Email);
    }

    /// <summary>
    /// Configura o ViewModel para modo de edição
    /// </summary>
    public async Task ConfigurarEdicaoAsync(int contatoId)
    {
        await ExecutarSeguroAsync(async () =>
        {
            var contato = await _agendaService.ObterContatoPorIdAsync(contatoId);
            if (contato != null)
            {
                Contato = contato;
                IsEdicao = true;
                TituloPagina = "Editar Contato";
                TextoBotao = "Atualizar";
            }
            else
            {
                await ExibirAlertaAsync("Erro", "Contato não encontrado.");
                await Shell.Current.GoToAsync("..");
            }
        }, "Erro ao carregar contato");
    }

    /// <summary>
    /// Configura o ViewModel para modo de criação
    /// </summary>
    public void ConfigurarCriacao()
    {
        Contato = new Contato();
        IsEdicao = false;
        TituloPagina = "Novo Contato";
        TextoBotao = "Salvar";
    }

    #endregion
}