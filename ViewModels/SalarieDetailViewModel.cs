using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure; // <-- C'est cette ligne qui manquait !

namespace AnnuaireEntreprise.ViewModels;

[QueryProperty(nameof(SalarieId), "SalarieId")]
public class SalarieDetailViewModel : BindableObject
{
    private readonly AppDbContext _db;
    private Salarie _salarie;

    public Salarie Salarie { get => _salarie; set { _salarie = value; OnPropertyChanged(); } }
    
    private string _salarieId;
    public string SalarieId { get => _salarieId; set { _salarieId = value; ChargerSalarie(value); } }

    public ICommand CallCommand { get; }
    public ICommand EmailCommand { get; }
    public ICommand GeneratePdfCommand { get; }

    public SalarieDetailViewModel()
    {
        _db = new AppDbContext();
        CallCommand = new Command<string>((phone) => PhoneDialer.Default.Open(phone));
        EmailCommand = new Command<string>((email) => Email.Default.ComposeAsync(new EmailMessage { To = { email } }));
        GeneratePdfCommand = new Command(async () => await GenererFichePdf());
    }

    private async void ChargerSalarie(string id)
    {
        if (int.TryParse(id, out int realId))
            Salarie = await _db.Salaries.Include(s => s.Service).Include(s => s.Site).FirstOrDefaultAsync(s => s.Id == realId);
    }

    private async Task GenererFichePdf()
    {
        if (Salarie == null) return;
        try
        {
            string fileName = $"Fiche_{Salarie.Nom}_{Salarie.Prenom}.pdf";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            Document.Create(container => {
                container.Page(page => {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Header().Text("Fiche Salarié").SemiBold().FontSize(28);
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col => {
                        col.Item().Text($"{Salarie.Nom} {Salarie.Prenom}").FontSize(20).SemiBold();
                        col.Item().Text($"🏢 Service : {Salarie.Service?.Nom}");
                        col.Item().Text($"📍 Site : {Salarie.Site?.Ville}");
                        col.Item().Text($"📞 Tel : {Salarie.TelephoneFixe} | Port : {Salarie.TelephonePortable}");
                        col.Item().Text($"✉️ Email : {Salarie.Email}");
                    });
                });
            }).GeneratePdf(filePath);

            await Launcher.Default.OpenAsync(new OpenFileRequest("Ouvrir la fiche PDF", new ReadOnlyFile(filePath)));
        }
        catch (Exception ex) { await Shell.Current.DisplayAlert("Erreur", ex.Message, "OK"); }
    }
}