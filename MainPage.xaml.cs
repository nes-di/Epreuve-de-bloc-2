using AnnuaireEntreprise.ViewModels;

namespace AnnuaireEntreprise.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        
        // Note : Si tu as ajouté <viewmodels:MainViewModel /> dans ton fichier XAML,
        // cette ligne est techniquement en double, mais elle ne fait pas de mal.
        BindingContext = new MainViewModel();
    }

    // Cette méthode se déclenche à chaque fois que la page apparaît à l'écran
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // On demande au ViewModel de rafraîchir les données
        if (BindingContext is MainViewModel vm)
        {
            _ = vm.ChargerDonneesAsync();
        }
    }
}