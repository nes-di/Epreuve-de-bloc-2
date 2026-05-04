using System.Collections.ObjectModel;
using System.Windows.Input;
using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Data;
using Microsoft.EntityFrameworkCore;
using AnnuaireEntreprise.Services;

namespace AnnuaireEntreprise.ViewModels;

public class AdminViewModel : BindableObject
{
    private readonly AppDbContext _db;
    private ObservableCollection<Salarie> _salaries = new();

    public ObservableCollection<Salarie> Salaries
    {
        get => _salaries;
        set { _salaries = value; OnPropertyChanged(); }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ImportApiCommand { get; }

    public AdminViewModel()
    {
        _db = new AppDbContext();
        AddCommand = new Command(async () => await Shell.Current.GoToAsync("SalarieFormPage"));
        EditCommand = new Command<Salarie>(async (s) => await Shell.Current.GoToAsync($"SalarieFormPage?SalarieId={s.Id}"));
        DeleteCommand = new Command<Salarie>(async (s) => await SupprimerSalarie(s));
        ImportApiCommand = new Command(async () => await ExecuterImportApi());

        _ = ChargerSalaries();
    }

    public async Task ChargerSalaries()
    {
        var liste = await _db.Salaries.Include(s => s.Service).Include(s => s.Site).ToListAsync();
        MainThread.BeginInvokeOnMainThread(() => Salaries = new ObservableCollection<Salarie>(liste));
    }

    private async Task ExecuterImportApi()
    {
        await DatabaseSeeder.Import10UsersAsync(_db);
        await ChargerSalaries();
        await Shell.Current.DisplayAlert("Succès", "10 salariés importés !", "OK");
    }

    private async Task SupprimerSalarie(Salarie salarie)
    {
        bool answer = await Shell.Current.DisplayAlert("Confirmation", $"Supprimer {salarie.Prenom} {salarie.Nom} ?", "Oui", "Non");
        if (answer)
        {
            _db.Salaries.Remove(salarie);
            await _db.SaveChangesAsync();
            await ChargerSalaries(); 
        }
    }
}