using AnnuaireEntreprise.Views;

namespace AnnuaireEntreprise;

public partial class AppShell : Shell
{
    private bool _isAdmin;
    public bool IsAdmin
    {
        get => _isAdmin;
        set 
        { 
            _isAdmin = value; 
            OnPropertyChanged(); // Notifie le XAML pour mettre à jour la visibilité
        }
    }

    public AppShell()
    {
        InitializeComponent();
        
        // On lie le Shell à lui-même pour que le binding {Binding IsAdmin} fonctionne
        BindingContext = this;

        // Enregistrement des routes
        Routing.RegisterRoute("SalarieDetailPage", typeof(SalarieDetailPage));
        Routing.RegisterRoute("SalarieFormPage", typeof(SalarieFormPage));
        Routing.RegisterRoute("ServicesPage", typeof(ServicesPage));
        Routing.RegisterRoute("SitesPage", typeof(SitesPage));
        Routing.RegisterRoute("AdminPage", typeof(AdminPage));
    }

    // Cette méthode intercepte le clic sur "Déconnexion" avant qu'il ne change de page
    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        if (args.Target.Location.OriginalString.Contains("déconnexion_clic"))
        {
            // On annule la navigation automatique
            args.Cancel();

            // On demande confirmation
            bool answer = await DisplayAlert("Déconnexion", "Voulez-vous vraiment vous déconnecter ?", "Oui", "Non");
            
            if (answer)
            {
                MainThread.BeginInvokeOnMainThread(async () => 
                {
                    IsAdmin = false;
                    await Shell.Current.GoToAsync("//MainPage");
                });
            }
        }
    }

    // On garde la méthode publique pour le LoginViewModel
    public void RefreshMenu()
    {
        OnPropertyChanged(nameof(IsAdmin));
    }
}