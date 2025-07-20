// Copyright © 2025 xbabco. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.OCR;

namespace MaestralMauiApp.PageModels;

public partial class TextRecognitionImagePageModel : ObservableObject
{
    private readonly IOcrService _ocr;

    [ObservableProperty]
    public partial ImageSource SelectedImage { get; set; } = "sample_image.jpg";

    [ObservableProperty]
    public partial RecognizedTextResult? RecognizedTextResult { get; set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    public TextRecognitionImagePageModel(IOcrService ocr)
    {
        _ocr = ocr;
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
                SelectedImage = ImageSource.FromFile(fileResult.FullPath);
            }
        }
        catch (Exception)
        {
            // Handle exception
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
                return;
            }

            var result = await _ocr.RecognizeTextAsync(imageData).ConfigureAwait(false);
            if (result.Success)
            {
                RecognizedTextResult = ConvertOcrResultToRecognizedTextResult(result);
            }
        }
        catch (Exception)
        {
            // Handle exception
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

    private static RecognizedTextResult ConvertOcrResultToRecognizedTextResult(OcrResult ocrResult)
    {
        var recognizedResult = new RecognizedTextResult(ocrResult.AllText)
        {
            Blocks = ocrResult
                .Elements.Select(e => new RecognizedTextBlock
                {
                    Text = e.Text,
                    BoundingBox = new(e.X, e.Y, e.Width, e.Height),
                })
                .ToList(),
        };

        return recognizedResult;
    }
}
