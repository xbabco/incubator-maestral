using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.OCR;
using Syncfusion.Maui.Toolkit.Internals;
using System.Diagnostics;
using System.Reflection;
using System.Resources;

namespace MaestralMauiApp.PageModels;

public partial class ImageSelectorPageModel : ObservableObject
{
    private readonly IOcrService _ocr;

    public ImageSelectorPageModel(IOcrService ocr)
    {
        _ocr = ocr;
        _ocr.InitAsync();
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

            SelectedImage = ImageSource.FromFile(fileResult.FullPath);
        }
        catch (Exception ex)
        {
            await AppShell.DisplayToastAsync($"Error selecting image: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task TextRecognition()
    {
        IsBusy = true;
        try
        {
            var imageData = await GetImageDataAsync(SelectedImage);
            if (imageData == null)
            {
                await AppShell.DisplayToastAsync("Could not read image data.");
                return;
            }

            var result = await _ocr.RecognizeTextAsync(imageData);

            if (result.Success)
            {
                await AppShell.DisplayToastAsync("Text recognized!");
                Debug.WriteLine(result.AllText);
                Console.WriteLine(result.AllText);
            }
            else
            {
                await AppShell.DisplayToastAsync("No text found or an error occurred.");
            }
        }
        catch (Exception ex)
        {
            await AppShell.DisplayToastAsync($"Error during text recognition: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<byte[]?> GetImageDataAsync(ImageSource imageSource)
    {
        Stream? stream = null;
        try
        {
            if (imageSource is FileImageSource fileSource)
            {
                if (Path.IsPathRooted(fileSource.File))
                {
                    stream = File.OpenRead(fileSource.File);
                }
                else
                {
                    stream = await FileSystem.OpenAppPackageFileAsync(fileSource.File);
                    //using var memoryStream = new MemoryStream();
                    //await stream.CopyToAsync(memoryStream);
                    //byte[] imageBytes = memoryStream.ToArray();
                }
            }
            else if (imageSource is StreamImageSource streamSource)
            {
                stream = await streamSource.Stream(CancellationToken.None);
            }

            if (stream == null)
            {
                return null;
            }

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
        finally
        {
            stream?.Dispose();
        }
    }
}

