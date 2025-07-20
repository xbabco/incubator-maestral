// Copyright © 2025 xbabco. All rights reserved.

using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.OCR;

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
            var fileResult = await FilePicker
                .Default.PickAsync(
                    new PickOptions
                    {
                        PickerTitle = "Select an image",
                        FileTypes = FilePickerFileType.Images,
                    }
                )
                .ConfigureAwait(false);
            if (fileResult == null)
            {
                return;
            }

            SelectedImage = ImageSource.FromFile(fileResult.FullPath);
        }
        catch (Exception ex)
        {
            await AppShell
                .DisplayToastAsync($"Error selecting image: {ex.Message}")
                .ConfigureAwait(false);
        }
    }

    [RelayCommand]
    private async Task TextRecognition()
    {
        IsBusy = true;
        try
        {
            var imageData = await GetImageDataAsync(SelectedImage).ConfigureAwait(false);
            if (imageData == null)
            {
                await AppShell
                    .DisplayToastAsync("Could not read image data.")
                    .ConfigureAwait(false);
                return;
            }

            var result = await _ocr.RecognizeTextAsync(imageData).ConfigureAwait(false);

            if (result.Success)
            {
                await AppShell.DisplayToastAsync("Text recognized!").ConfigureAwait(false);
                Debug.WriteLine(result.AllText);
                Console.WriteLine(result.AllText);
            }
            else
            {
                await AppShell
                    .DisplayToastAsync("No text found or an error occurred.")
                    .ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            await AppShell
                .DisplayToastAsync($"Error during text recognition: {ex.Message}")
                .ConfigureAwait(false);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static async Task<byte[]?> GetImageDataAsync(ImageSource imageSource)
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
                    stream = await FileSystem
                        .OpenAppPackageFileAsync(fileSource.File)
                        .ConfigureAwait(false);
                    //using var memoryStream = new MemoryStream();
                    //await stream.CopyToAsync(memoryStream);
                    //byte[] imageBytes = memoryStream.ToArray();
                }
            }
            else if (imageSource is StreamImageSource streamSource)
            {
                stream = await streamSource.Stream(CancellationToken.None).ConfigureAwait(false);
            }

            if (stream == null)
            {
                return null;
            }

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
            return memoryStream.ToArray();
        }
        finally
        {
            stream?.Dispose();
        }
    }
}
