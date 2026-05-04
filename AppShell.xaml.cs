using AnnuaireEntreprise.Views;

namespace AnnuaireEntreprise;

public partial class AppShell : Shell
{
    private bool _isAdmin;
    public bool IsAdmin
    {
        get => _isAdmin;
        set { _isAdmin = value; OnPropertyChanged(); }
    }

    public AppShell()
    {
        InitializeComponent();
        BindingContext = this;

        Routing.RegisterRoute("SalarieDetailPage", typeof(SalarieDetailPage));
        Routing.RegisterRoute("SalarieFormPage", typeof(SalarieFormPage));
        Routing.RegisterRoute("ServicesPage", typeof(ServicesPage));
        Routing.RegisterRoute("SitesPage", typeof(SitesPage));
        Routing.RegisterRoute("AdminPage", typeof(AdminPage));
        Routing.RegisterRoute("LoginPage", typeof(LoginPage)); // <-- Route ajoutée ici !
    }

    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);
        if (args.Target.Location.OriginalString.Contains("déconnexion_clic"))
        {
            args.Cancel();
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

    public void RefreshMenu() => OnPropertyChanged(nameof(IsAdmin));
}