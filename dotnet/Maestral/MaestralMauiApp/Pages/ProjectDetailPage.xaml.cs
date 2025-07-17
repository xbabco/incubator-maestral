using MaestralMauiApp.Models;

namespace MaestralMauiApp.Pages
{
    public partial class ProjectDetailPage : ContentPage
    {
        public ProjectDetailPage(ProjectDetailPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
