using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace MaestralMauiApp.PageModels;

public partial class TextRecognitionImagePageModel : ObservableObject
{
    [ObservableProperty]
    private ImageSource _selectedImage = "sample_image.jpg";

    [RelayCommand]
    private async Task SelectImage()
    {
        try
        {
            var fileResult = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select an image",
                FileTypes = FilePickerFileType.Images
            });
            if (fileResult != null)
            {
                using var stream = await fileResult.OpenReadAsync();
                var imageStream = new MemoryStream();
                await stream.CopyToAsync(imageStream);
                imageStream.Position = 0;
                SelectedImage = ImageSource.FromStream(() => imageStream);
            }
        }
        catch
        {
        }
    }
}
