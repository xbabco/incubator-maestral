using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MaestralMauiApp.PageModels;

public partial class ImageSelectorPageModel : ObservableObject
{
    private readonly ITextRecognizer _textRecognizer;

    public ImageSelectorPageModel(ITextRecognizer textRecognizer)
    {
        _textRecognizer = textRecognizer;
    }

    [ObservableProperty]
    private ImageSource _selectedImage = "sample_image.jpg";

    [ObservableProperty]
    private bool _isBusy = false;

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
            if (fileResult == null)
            {
                return;
            }
            //using var stream = await fileResult.OpenReadAsync();
            //var imageStream = new MemoryStream();
            //await stream.CopyToAsync(imageStream);
            //imageStream.Position = 0;
            //SelectedImage = ImageSource.FromStream(() => imageStream);
            SelectedImage = ImageSource.FromFile(fileResult.FullPath);
        }
        catch
        {
        }
    }

    [RelayCommand]
    private async Task TextRecognition()
    {
        IsBusy = true;
        var result = SelectedImage switch
        {
            FileImageSource fileImageSource =>
                await _textRecognizer.RecognizeTextAsync(File.OpenRead(fileImageSource.File)),
            StreamImageSource streamImageSource =>
                await _textRecognizer.RecognizeTextAsync(await streamImageSource.Stream(CancellationToken.None)),
            _ => null
        };
        IsBusy = false;

        if (result == null)
        {
            await AppShell.DisplayToastAsync("No text found");
            return;
        }
        Console.WriteLine(result);

    }

    /*
List<Text.TextBlock> blocks = texts.getTextBlocks();
    if (blocks.size() == 0) {
        showToast("No text found");
        return;
    }
    mGraphicOverlay.clear();
    for (int i = 0; i < blocks.size(); i++) {
        List<Text.Line> lines = blocks.get(i).getLines();
        for (int j = 0; j < lines.size(); j++) {
            List<Text.Element> elements = lines.get(j).getElements();
            for (int k = 0; k < elements.size(); k++) {
                Graphic textGraphic = new TextGraphic(mGraphicOverlay, elements.get(k));
                mGraphicOverlay.add(textGraphic);

            }
        }
    }
     */
}
