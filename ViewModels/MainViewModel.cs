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

        // État du rafraîchissement (le petit rond qui tourne)
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set { _isRefreshing = value; OnPropertyChanged(); }
        }

        // Commandes pour l'UI
        public ICommand RefreshCommand { get; }

        public MainViewModel()
        {
            _db = new AppDbContext();
            RefreshCommand = new Command(async () => await ChargerDonneesAsync());
            
            // Chargement initial
            Task.Run(async () => await ChargerDonneesAsync());
        }

        private async Task ChargerDonneesAsync()
        {
            IsRefreshing = true;

            try
            {
                // On récupère les salariés avec leurs relations (Join SQL)
                var liste = await _db.Salaries
                    .Include(s => s.Service)
                    .Include(s => s.Site)
                    .OrderBy(s => s.Nom)
                    .ToListAsync();

                // On met à jour l'UI sur le thread principal
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