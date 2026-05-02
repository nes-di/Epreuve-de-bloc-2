using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AnnuaireEntreprise.ViewModels
{
    // INotifyPropertyChanged est indispensable en MVVM :
    // C'est ce qui permet de dire à l'interface graphique "Hey, une donnée a changé, mets-toi à jour !"
    public class MainViewModel : INotifyPropertyChanged
    {
        private AppDbContext _db;
        private ObservableCollection<Salarie> _salaries;
        private string _rechercheTexte;

        // ObservableCollection est une liste spéciale pour MAUI. 
        // Si on y ajoute ou supprime un élément, l'interface se met à jour toute seule.
        public ObservableCollection<Salarie> Salaries
        {
            get => _salaries;
            set
            {
                _salaries = value;
                OnPropertyChanged(); // On prévient l'UI du changement
            }
        }

        // Le texte tapé par l'utilisateur dans la barre de recherche
        public string RechercheTexte
        {
            get => _rechercheTexte;
            set
            {
                _rechercheTexte = value;
                OnPropertyChanged();
                // A chaque lettre tapée, on lance la recherche (exigence du sujet)
                FiltrerSalaries();
            }
        }

        public MainViewModel()
        {
            _db = new AppDbContext();
            // On charge tous les salariés au démarrage (en incluant leurs infos de Service et Site)
            ChargerTousLesSalaries();
        }

        private void ChargerTousLesSalaries()
        {
            var tousLesSalaries = _db.Salaries
                                     .Include(s => s.Service)
                                     .Include(s => s.Site)
                                     .ToList();
            
            Salaries = new ObservableCollection<Salarie>(tousLesSalaries);
        }

        private void FiltrerSalaries()
        {
            if (string.IsNullOrWhiteSpace(RechercheTexte))
            {
                // Si la barre est vide, on remet tout
                ChargerTousLesSalaries();
                return;
            }

            // On filtre par nom ou prénom en ignorant la casse
            string recherche = RechercheTexte.ToLower();
            var resultats = _db.Salaries
                               .Include(s => s.Service)
                               .Include(s => s.Site)
                               .Where(s => s.Nom.ToLower().Contains(recherche) || 
                                           s.Prenom.ToLower().Contains(recherche))
                               .ToList();

            Salaries = new ObservableCollection<Salarie>(resultats);
        }

        // --- Logique obligatoire pour INotifyPropertyChanged ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}