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

        // On lance le remplissage de la base de données en tâche de fond
        // pour ne pas bloquer l'affichage de l'application
        Task.Run(async () => 
        {
            using var db = new AppDbContext();
            await DatabaseSeeder.SeedDataAsync(db);
        });
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}