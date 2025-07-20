// Copyright © 2025 xbabco. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.OCR;

namespace MaestralMauiApp.PageModels;

public partial class TextRecognitionImagePageModel : ObservableObject
{
    private readonly IOcrService _ocr;

    [ObservableProperty]
    private ImageSource _selectedImage = "sample_image.jpg";

    [ObservableProperty]
    private string? _recognizedText;

    public TextRecognitionImagePageModel(IOcrService ocr)
    {
        _ocr = ocr;
        _ocr.InitAsync();
    }

    [RelayCommand]
    private async Task SelectImage()
    {
        try
        {
            var fileResult = await FilePicker
                .Default.PickAsync(
                    new PickOptions
                    {
                        PickerTitle = "Select an image",
                        FileTypes = FilePickerFileType.Images,
                    }
                )
                .ConfigureAwait(false);
            if (fileResult != null)
            {
                using var stream = await fileResult.OpenReadAsync().ConfigureAwait(false);
                var imageStream = new MemoryStream();
                await stream.CopyToAsync(imageStream).ConfigureAwait(false);
                imageStream.Position = 0;
                SelectedImage = ImageSource.FromStream(() => imageStream);

                imageStream.Position = 0;
                var ocrResult = await _ocr.RecognizeTextAsync(imageStream.ToArray())
                    .ConfigureAwait(false);
                if (ocrResult.Success)
                {
                    RecognizedText = ocrResult.AllText;
                }
            }
        }
        catch { }
    }
}
