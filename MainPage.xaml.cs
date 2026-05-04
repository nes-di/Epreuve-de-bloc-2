using AnnuaireEntreprise.ViewModels;

namespace AnnuaireEntreprise.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MainViewModel vm)
        {
            _ = vm.ChargerDonneesAsync();
        }
    }

    // On intercepte le moment où la page MAUI s'affiche sur Windows
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

#if WINDOWS
        // On récupère le contrôle natif Windows de l'application
        if (Handler?.PlatformView is Microsoft.UI.Xaml.UIElement nativeView)
        {
            // On crée un raccourci clavier invisible directement dans Windows
            var hiddenShortcut = new Microsoft.UI.Xaml.Input.KeyboardAccelerator()
            {
                Key = Windows.System.VirtualKey.A,
                Modifiers = Windows.System.VirtualKeyModifiers.Control | Windows.System.VirtualKeyModifiers.Shift
            };

            // Quand les touches sont pressées, on déclenche la commande d'ouverture de la page de login
            hiddenShortcut.Invoked += (sender, args) =>
            {
                if (BindingContext is MainViewModel vm)
                {
                    vm.OpenHiddenLoginCommand.Execute(null);
                    args.Handled = true; // On bloque l'événement pour éviter les bugs
                }
            };

            // On l'attache à la page
            nativeView.KeyboardAccelerators.Add(hiddenShortcut);
        }
#endif
    }
}