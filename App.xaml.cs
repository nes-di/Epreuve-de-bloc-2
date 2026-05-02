using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using AnnuaireEntreprise.Data;
using AnnuaireEntreprise.Services;

namespace AnnuaireEntreprise;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // FORCE l'utilisation du Shell comme page principale
        MainPage = new AppShell();

        // On lance le remplissage de la base de données en tâche de fond
        Task.Run(async () => 
        {
            using var db = new AppDbContext();
            await DatabaseSeeder.SeedDataAsync(db);
        });
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // On s'assure que la fenêtre utilise bien le Shell défini plus haut
        return new Window(MainPage);
    }
}