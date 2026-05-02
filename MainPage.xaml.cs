using AnnuaireEntreprise.ViewModels;

namespace AnnuaireEntreprise.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        
        // On connecte la Vue à son "Cerveau" !
        BindingContext = new MainViewModel();
    }
}