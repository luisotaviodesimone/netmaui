using CommunityToolkit.Mvvm.ComponentModel;

namespace AppAgenda.MVVM.ViewModels;

/// <summary>
/// ViewModel base com funcionalidades comuns
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy = false;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _mensagem = string.Empty;

    /// <summary>
    /// Exibe uma mensagem de alerta para o usuário
    /// </summary>
    protected async Task ExibirAlertaAsync(string titulo, string mensagem, string botao = "OK")
    {
        await Application.Current?.MainPage?.DisplayAlert(titulo, mensagem, botao);
    }

    /// <summary>
    /// Exibe uma mensagem de confirmação para o usuário
    /// </summary>
    protected async Task<bool> ExibirConfirmacaoAsync(string titulo, string mensagem, string aceitar = "Sim", string cancelar = "Não")
    {
        return await Application.Current?.MainPage?.DisplayAlert(titulo, mensagem, aceitar, cancelar);
    }

    /// <summary>
    /// Executa uma ação de forma segura com tratamento de erro
    /// </summary>
    protected async Task ExecutarSeguroAsync(Func<Task> acao, string? mensagemErro = null)
    {
        try
        {
            IsBusy = true;
            await acao();
        }
        catch (Exception ex)
        {
            Mensagem = mensagemErro ?? ex.Message;
            await ExibirAlertaAsync("Erro", Mensagem);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Executa uma ação com retorno de forma segura com tratamento de erro
    /// </summary>
    protected async Task<T?> ExecutarSeguroAsync<T>(Func<Task<T>> acao, string? mensagemErro = null)
    {
        try
        {
            IsBusy = true;
            return await acao();
        }
        catch (Exception ex)
        {
            Mensagem = mensagemErro ?? ex.Message;
            await ExibirAlertaAsync("Erro", Mensagem);
            return default(T);
        }
        finally
        {
            IsBusy = false;
        }
    }
}