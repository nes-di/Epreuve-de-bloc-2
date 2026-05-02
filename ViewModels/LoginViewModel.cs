using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AnnuaireEntreprise.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;

    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new Command(async () => await ExecuterLogin());
    }

    private async Task ExecuterLogin()
    {
        // Tu peux changer "admin123" par le mot de passe de ton choix
        if (Password == "admin123") 
        {
            ErrorMessage = string.Empty;
            // On navigue vers la future page d'admin
            await Shell.Current.GoToAsync("//AdminPage"); 
        }
        else
        {
            ErrorMessage = "Mot de passe incorrect !";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}