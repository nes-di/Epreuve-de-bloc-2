using System.Collections.ObjectModel;
using System.Windows.Input;
using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Data;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireEntreprise.ViewModels;

public class SitesViewModel : BindableObject
{
    private readonly AppDbContext _db;
    private ObservableCollection<Site> _sites = new();
    private string _nouvelleVille = string.Empty;

    public ObservableCollection<Site> Sites 
    { 
        get => _sites; 
        set { _sites = value; OnPropertyChanged(); } 
    }

    public string NouvelleVille 
    { 
        get => _nouvelleVille; 
        set { _nouvelleVille = value; OnPropertyChanged(); } 
    }

    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }

    public SitesViewModel()
    {
        _db = new AppDbContext();
        AddCommand = new Command(async () => await AjouterSite());
        DeleteCommand = new Command<Site>(async (s) => await SupprimerSite(s));
        _ = ChargerSites();
    }

    public async Task ChargerSites()
    {
        var liste = await _db.Sites.ToListAsync();
        Sites = new ObservableCollection<Site>(liste);
    }

    private async Task AjouterSite()
    {
        if (string.IsNullOrWhiteSpace(NouvelleVille)) return;

        _db.Sites.Add(new Site { Ville = NouvelleVille });
        await _db.SaveChangesAsync();
        
        NouvelleVille = string.Empty;
        await ChargerSites();
    }

    private async Task SupprimerSite(Site site)
    {
        // On vérifie si des salariés travaillent sur ce site avant de supprimer
        var count = await _db.Salaries.CountAsync(s => s.SiteId == site.Id);
        if (count > 0)
        {
            await Shell.Current.DisplayAlert("Attention", "Des salariés sont encore affectés à ce site.", "OK");
            return;
        }

        _db.Sites.Remove(site);
        await _db.SaveChangesAsync();
        await ChargerSites();
    }
}