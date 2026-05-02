using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireEntreprise.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private ObservableCollection<Salarie> _salaries = new();
        private string _rechercheTexte = string.Empty;
        private bool _isRefreshing;

        // Liste des salariés affichés à l'écran
        public ObservableCollection<Salarie> Salaries
        {
            get => _salaries;
            set { _salaries = value; OnPropertyChanged(); }
        }

        // Texte de la barre de recherche
        public string RechercheTexte
        {
            get => _rechercheTexte;
            set 
            { 
                _rechercheTexte = value; 
                OnPropertyChanged(); 
                FiltrerSalaries(); 
            }
        }

        // État du rafraîchissement
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set { _isRefreshing = value; OnPropertyChanged(); }
        }

        // Commandes pour l'UI
        public ICommand RefreshCommand { get; }
        public ICommand GoToDetailCommand { get; } // <-- NOUVEAU

        public MainViewModel()
        {
            _db = new AppDbContext();
            
            // Initialisation de la commande de rafraîchissement
            RefreshCommand = new Command(async () => await ChargerDonneesAsync());

            // Initialisation de la commande de navigation vers le détail
            GoToDetailCommand = new Command<Salarie>(async (salarie) =>
            {
                if (salarie == null) return;
                
                // On navigue vers la page de détail en passant l'ID
                await Shell.Current.GoToAsync($"SalarieDetailPage?SalarieId={salarie.Id}");
            });
            
            // Chargement initial
            _ = ChargerDonneesAsync();
        }

        // On le passe en public pour pouvoir l'appeler depuis le OnAppearing de la vue si besoin
        public async Task ChargerDonneesAsync()
        {
            if (IsRefreshing && Salaries.Count > 0) return; // Évite les doubles appels

            IsRefreshing = true;

            try
            {
                var liste = await _db.Salaries
                    .Include(s => s.Service)
                    .Include(s => s.Site)
                    .OrderBy(s => s.Nom)
                    .ToListAsync();

                MainThread.BeginInvokeOnMainThread(() => {
                    Salaries = new ObservableCollection<Salarie>(liste);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Erreur] Impossible de charger les données : {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private void FiltrerSalaries()
        {
            if (string.IsNullOrWhiteSpace(RechercheTexte))
            {
                _ = ChargerDonneesAsync();
                return;
            }

            var filtre = RechercheTexte.ToLower();
            var resultats = _db.Salaries
                .Include(s => s.Service)
                .Include(s => s.Site)
                .Where(s => s.Nom.ToLower().Contains(filtre) || 
                            s.Prenom.ToLower().Contains(filtre))
                .ToList();

            Salaries = new ObservableCollection<Salarie>(resultats);
        }

        // Logique MVVM standard
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}