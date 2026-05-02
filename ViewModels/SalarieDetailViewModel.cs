using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace AnnuaireEntreprise.ViewModels;

[QueryProperty(nameof(SalarieId), "SalarieId")]
public class SalarieDetailViewModel : BindableObject
{
    private readonly AppDbContext _db;
    private Salarie _salarie;

    public Salarie Salarie { get => _salarie; set { _salarie = value; OnPropertyChanged(); } }

    private string _salarieId;
    public string SalarieId
    {
        get => _salarieId;
        set { _salarieId = value; ChargerSalarie(value); }
    }

    public ICommand CallCommand { get; }
    public ICommand EmailCommand { get; }

    public SalarieDetailViewModel()
    {
        _db = new AppDbContext();
        CallCommand = new Command<string>((phone) => PhoneDialer.Default.Open(phone));
        EmailCommand = new Command<string>((email) => Email.Default.ComposeAsync(new EmailMessage { To = { email } }));
    }

    private async void ChargerSalarie(string id)
    {
        if (int.TryParse(id, out int realId))
        {
            Salarie = await _db.Salaries
                .Include(s => s.Service)
                .Include(s => s.Site)
                .FirstOrDefaultAsync(s => s.Id == realId);
        }
    }
}