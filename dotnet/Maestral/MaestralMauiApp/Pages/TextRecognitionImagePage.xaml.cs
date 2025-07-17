namespace MaestralMauiApp.Pages
{
    public partial class TextRecognitionImagePage : ContentPage
    {
        public TextRecognitionImagePage(TextRecognitionImagePageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
