using Ascolta.ViewModels;

namespace Ascolta.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm; // Se usa el VM inyectado por DI
        }
    }
}
