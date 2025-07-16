using MaestralMauiApp.Models;
using MaestralMauiApp.PageModels;

namespace MaestralMauiApp.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}