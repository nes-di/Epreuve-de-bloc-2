using System.Collections.ObjectModel;
using System.Windows.Input;
using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Data;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireEntreprise.ViewModels;

public class ServicesViewModel : BindableObject
{
    private readonly AppDbContext _db;
    private ObservableCollection<Service> _services = new();
    private string _nouveauNom = string.Empty;

    public ObservableCollection<Service> Services 
    { 
        get => _services; 
        set { _services = value; OnPropertyChanged(); } 
    }

    public string NouveauNom 
    { 
        get => _nouveauNom; 
        set { _nouveauNom = value; OnPropertyChanged(); } 
    }

    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }

    public ServicesViewModel()
    {
        _db = new AppDbContext();
        AddCommand = new Command(async () => await AjouterService());
        DeleteCommand = new Command<Service>(async (s) => await SupprimerService(s));
        _ = ChargerServices();
    }

    public async Task ChargerServices()
    {
        var liste = await _db.Services.ToListAsync();
        Services = new ObservableCollection<Service>(liste);
    }

    private async Task AjouterService()
    {
        if (string.IsNullOrWhiteSpace(NouveauNom)) return;

        _db.Services.Add(new Service { Nom = NouveauNom });
        await _db.SaveChangesAsync();
        
        NouveauNom = string.Empty; // Reset le champ
        await ChargerServices();   // Rafraîchit la liste
    }

    private async Task SupprimerService(Service service)
    {
        // Vérification si des salariés sont liés (pour éviter les erreurs SQL)
        var count = await _db.Salaries.CountAsync(s => s.ServiceId == service.Id);
        if (count > 0)
        {
            await Shell.Current.DisplayAlert("Action impossible", "Ce service contient des salariés. Déplacez-les d'abord.", "OK");
            return;
        }

        _db.Services.Remove(service);
        await _db.SaveChangesAsync();
        await ChargerServices();
    }
}