// Copyright © 2025 xbabco. All rights reserved.

using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.OCR;

namespace MaestralMauiApp.PageModels;

public partial class ImageSelectorPageModel(IOcrService ocr) : ObservableObject
{
    private readonly IOcrService _ocr = ocr;

    [ObservableProperty]
    public partial RecognizedTextResult? RecognizedTextResult { get; set; } = null;

    [ObservableProperty]
    public partial ImageSource SelectedImage { get; set; } = "sample_image.jpg";

    [ObservableProperty]
    public partial bool IsBusy { get; set; } = false;

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
                .ConfigureAwait(true);
            if (fileResult == null)
            {
                return;
            }

            SelectedImage = ImageSource.FromFile(fileResult.FullPath);
            RecognizedTextResult = null;
        }
        catch (Exception ex)
        {
            await AppShell
                .DisplayToastAsync($"Error selecting image: {ex.Message}")
                .ConfigureAwait(true);
        }
    }

    [RelayCommand]
    private async Task RecognizeText()
    {
        IsBusy = true;
        try
        {
            var imageData = await GetImageDataAsync(SelectedImage).ConfigureAwait(true);
            if (imageData == null)
            {
                await AppShell.DisplayToastAsync("Could not read image data.").ConfigureAwait(true);
                return;
            }
            var b = new OcrOptions.Builder();
            b.SetTryHard(true);
            var options = b.Build();
            var result = await _ocr.RecognizeTextAsync(imageData, options).ConfigureAwait(true);

            if (result != null && result.Success)
            {
                await AppShell.DisplayToastAsync("Text recognized!").ConfigureAwait(true);
                Debug.WriteLine(result.AllText);
                Console.WriteLine(result.AllText);
            }
            else
            {
                await AppShell
                    .DisplayToastAsync("No text found or an error occurred.")
                    .ConfigureAwait(true);
            }
            RecognizedTextResult = ConvertOcrResultToRecognizedTextResult(result);
        }
        catch (Exception ex)
        {
            await AppShell
                .DisplayToastAsync($"Error during text recognition: {ex.Message}")
                .ConfigureAwait(true);
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
                    stream = await Task.Run(() => File.OpenRead(fileSource.File))
                        .ConfigureAwait(false);
                }
                else
                {
                    stream = await FileSystem
                        .OpenAppPackageFileAsync(fileSource.File)
                        .ConfigureAwait(false);
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

    private static RecognizedTextResult ConvertOcrResultToRecognizedTextResult(OcrResult? ocrResult)
    {
        var recognizedResult = new RecognizedTextResult(ocrResult?.AllText ?? string.Empty)
        {
            Blocks =
                ocrResult
                    ?.Elements?.Select(it => new RecognizedTextBlock
                    {
                        Text = it.Text ?? string.Empty,
                        BoundingBox = new RectF(it.X, it.Y, it.Width, it.Height),
                    })
                    .ToList() ?? [],
        };

        return recognizedResult;
    }
}
