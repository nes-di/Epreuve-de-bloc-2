using System.Collections.ObjectModel;
using System.Windows.Input;
using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Data;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireEntreprise.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private readonly AppDbContext _db;
        private ObservableCollection<Salarie> _salaries = new();
        private ObservableCollection<Site> _sites = new();
        private ObservableCollection<Service> _services = new();
        private bool _isRefreshing;
        private string _rechercheTexte = string.Empty;
        private Site _selectedSite;
        private Service _selectedService;

        public ObservableCollection<Salarie> Salaries { get => _salaries; set { _salaries = value; OnPropertyChanged(); } }
        public ObservableCollection<Site> Sites { get => _sites; set { _sites = value; OnPropertyChanged(); } }
        public ObservableCollection<Service> Services { get => _services; set { _services = value; OnPropertyChanged(); } }
        public bool IsRefreshing { get => _isRefreshing; set { _isRefreshing = value; OnPropertyChanged(); } }
        
        public string RechercheTexte { get => _rechercheTexte; set { _rechercheTexte = value; OnPropertyChanged(); FiltrerSalaries(); } }
        public Site SelectedSite { get => _selectedSite; set { _selectedSite = value; OnPropertyChanged(); FiltrerSalaries(); } }
        public Service SelectedService { get => _selectedService; set { _selectedService = value; OnPropertyChanged(); FiltrerSalaries(); } }

        public ICommand RefreshCommand { get; }
        public ICommand GoToDetailCommand { get; }
        public ICommand ResetFiltersCommand { get; }
        public ICommand OpenHiddenLoginCommand { get; } // <-- Commande ajoutée

        public MainViewModel()
        {
            _db = new AppDbContext();
            RefreshCommand = new Command(async () => await ChargerDonneesAsync());
            GoToDetailCommand = new Command<Salarie>(async (s) => await Shell.Current.GoToAsync($"SalarieDetailPage?SalarieId={s.Id}"));
            ResetFiltersCommand = new Command(() => { RechercheTexte = string.Empty; SelectedSite = null; SelectedService = null; _ = ChargerDonneesAsync(); });
            
            // <-- Initialisation de la commande pour le raccourci
            OpenHiddenLoginCommand = new Command(async () => await Shell.Current.GoToAsync("LoginPage")); 
            
            _ = ChargerDonneesAsync();
        }

        public async Task ChargerDonneesAsync()
        {
            IsRefreshing = true;
            var listeSalaries = await _db.Salaries.Include(s => s.Service).Include(s => s.Site).OrderBy(s => s.Nom).ToListAsync();
            MainThread.BeginInvokeOnMainThread(() => {
                Salaries = new ObservableCollection<Salarie>(listeSalaries);
                Sites = new ObservableCollection<Site>(_db.Sites.OrderBy(s => s.Ville).ToList());
                Services = new ObservableCollection<Service>(_db.Services.OrderBy(s => s.Nom).ToList());
            });
            IsRefreshing = false;
        }

        private void FiltrerSalaries()
        {
            var query = _db.Salaries.Include(s => s.Service).Include(s => s.Site).AsQueryable();

            if (!string.IsNullOrWhiteSpace(RechercheTexte))
                query = query.Where(s => s.Nom.ToLower().Contains(RechercheTexte.ToLower()) || s.Prenom.ToLower().Contains(RechercheTexte.ToLower()));
            
            if (SelectedSite != null) query = query.Where(s => s.SiteId == SelectedSite.Id);
            if (SelectedService != null) query = query.Where(s => s.ServiceId == SelectedService.Id);

            Salaries = new ObservableCollection<Salarie>(query.OrderBy(s => s.Nom).ToList());
        }
    }
}