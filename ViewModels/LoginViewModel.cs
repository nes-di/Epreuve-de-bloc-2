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
        // Vérification du mot de passe (à adapter selon tes besoins)
        if (Password == "admin123") 
        {
            ErrorMessage = string.Empty;

            // On récupère l'instance du Shell pour modifier son état
            if (Shell.Current is AppShell shell)
            {
                // Crucial pour Windows : On effectue les changements UI sur le thread principal 
                // pour éviter l'erreur de corruption mémoire (-1073741189)
                MainThread.BeginInvokeOnMainThread(async () => 
                {
                    // 1. On active le mode Admin
                    shell.IsAdmin = true; 
                    
                    // 2. On rafraîchit le menu (pour ajouter le bouton déconnexion)
                    shell.RefreshMenu();

                    // 3. On vide le champ pour la sécurité
                    Password = string.Empty;

                    // 4. On redirige vers l'administration
                    // L'utilisation de "//" réinitialise la pile de navigation
                    await Shell.Current.GoToAsync("//AdminPage"); 
                });
            }
        }
        else
        {
            ErrorMessage = "Mot de passe incorrect !";
        }
    }

    #region MVVM Standard
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    #endregion
}