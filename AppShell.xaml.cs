using AnnuaireEntreprise.Views;

namespace AnnuaireEntreprise;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Indispensable pour la navigation avec paramètres (ex: SalarieId)
        Routing.RegisterRoute("SalarieFormPage", typeof(SalarieFormPage));
        
        // On enregistre aussi les autres pages de gestion pour une navigation fluide
        Routing.RegisterRoute("ServicesPage", typeof(ServicesPage));
        Routing.RegisterRoute("SitesPage", typeof(SitesPage));
        Routing.RegisterRoute("AdminPage", typeof(AdminPage));
    }
}
