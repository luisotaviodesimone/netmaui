using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppAgenda.Models
{
  [Table("Contatos")]
  public class Contato : INotifyPropertyChanged
  {
    private int _id;
    private string _nome = string.Empty;
    private string _email = string.Empty;

    [PrimaryKey, AutoIncrement]
    public int Id
    {
      get => _id;
      set => SetProperty(ref _id, value);
    }

    [NotNull, MaxLength(100)]
    public string Nome
    {
      get => _nome;
      set => SetProperty(ref _nome, value);
    }

    [MaxLength(150)]
    public string Email
    {
      get => _email;
      set => SetProperty(ref _email, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
      if (Equals(storage, value))
        return false;

      storage = value;
      OnPropertyChanged(propertyName);
      return true;
    }

    // Validação de email simples
    public bool IsValidEmail()
    {
      if (string.IsNullOrWhiteSpace(Email))
        return false;

      try
      {
        var addr = new System.Net.Mail.MailAddress(Email);
        return addr.Address == Email;
      }
      catch
      {
        return false;
      }
    }

    // Validação geral do contato
    public bool IsValid()
    {
      return !string.IsNullOrWhiteSpace(Nome) &&
             Nome.Length <= 100 &&
             (string.IsNullOrWhiteSpace(Email) || IsValidEmail()) &&
             (string.IsNullOrWhiteSpace(Email) || Email.Length <= 150);
    }

    public override string ToString()
    {
      return $"{Nome} - {Email}";
    }
  }
}