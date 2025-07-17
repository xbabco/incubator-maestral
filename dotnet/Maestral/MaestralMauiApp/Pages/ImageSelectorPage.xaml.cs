using Microsoft.Maui.Controls;
using MaestralMauiApp.PageModels;

namespace MaestralMauiApp.Pages
{
    public partial class ImageSelectorPage : ContentPage
    {
        public ImageSelectorPage(ImageSelectorPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
