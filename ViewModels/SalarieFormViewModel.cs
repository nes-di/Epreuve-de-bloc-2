using System.Collections.ObjectModel;
using System.Windows.Input;
using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Data;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireEntreprise.ViewModels;

// Le QueryProperty permet de recevoir l'ID du salarié quand on clique sur "Modifier"
[QueryProperty(nameof(SalarieId), "SalarieId")]
public class SalarieFormViewModel : BindableObject
{
    private readonly AppDbContext _db;
    
    // Propriétés pour le formulaire
    public Salarie SalarieCourant { get; set; } = new();
    public ObservableCollection<Service> Services { get; set; } = new();
    public ObservableCollection<Site> Sites { get; set; } = new();

    private Service _selectedService;
    public Service SelectedService { get => _selectedService; set { _selectedService = value; OnPropertyChanged(); } }

    private Site _selectedSite;
    public Site SelectedSite { get => _selectedSite; set { _selectedSite = value; OnPropertyChanged(); } }

    private string _salarieId;
    public string SalarieId
    {
        get => _salarieId;
        set { _salarieId = value; ChargerSalarie(value); }
    }

    public ICommand SaveCommand { get; }

    public SalarieFormViewModel()
    {
        _db = new AppDbContext();
        SaveCommand = new Command(async () => await Enregistrer());
        _ = ChargerDonneesInitiales();
    }

    private async Task ChargerDonneesInitiales()
    {
        Services = new ObservableCollection<Service>(await _db.Services.ToListAsync());
        Sites = new ObservableCollection<Site>(await _db.Sites.ToListAsync());
        OnPropertyChanged(nameof(Services));
        OnPropertyChanged(nameof(Sites));
    }

    private async void ChargerSalarie(string id)
    {
        if (int.TryParse(id, out int realId))
        {
            var s = await _db.Salaries.Include(x => x.Service).Include(x => x.Site)
                             .FirstOrDefaultAsync(x => x.Id == realId);
            if (s != null)
            {
                SalarieCourant = s;
                SelectedService = Services.FirstOrDefault(x => x.Id == s.ServiceId);
                SelectedSite = Sites.FirstOrDefault(x => x.Id == s.SiteId);
                OnPropertyChanged(nameof(SalarieCourant));
            }
        }
    }

    private async Task Enregistrer()
    {
        if (SelectedService == null || SelectedSite == null) return;

        SalarieCourant.ServiceId = SelectedService.Id;
        SalarieCourant.SiteId = SelectedSite.Id;

        if (SalarieCourant.Id == 0) _db.Salaries.Add(SalarieCourant);
        else _db.Salaries.Update(SalarieCourant);

        await _db.SaveChangesAsync();
        await Shell.Current.GoToAsync(".."); // Retour à la page précédente
    }
}