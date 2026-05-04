using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AnnuaireEntreprise.Services;

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
        if (Password == "admin123") 
        {
            ErrorMessage = string.Empty;
            
            // On enregistre le log de succès
            LoggerService.Log("Succès : Connexion au mode Administrateur.");

            if (Shell.Current is AppShell shell)
            {
                MainThread.BeginInvokeOnMainThread(async () => 
                {
                    shell.IsAdmin = true; 
                    shell.RefreshMenu();
                    Password = string.Empty;
                    await Shell.Current.GoToAsync("//AdminPage"); 
                });
            }
        }
        else
        {
            ErrorMessage = "Mot de passe incorrect !";
            
            // On enregistre le log d'erreur
            LoggerService.Log($"Erreur : Tentative de connexion admin échouée avec le mot de passe '{Password}'.");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}