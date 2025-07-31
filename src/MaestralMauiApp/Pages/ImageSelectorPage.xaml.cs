// Copyright © 2025 xbabco. All rights reserved.

using System.ComponentModel;
using System.Diagnostics;
using MaestralMauiApp.Pages.Controls;
using Plugin.Maui.OCR;

namespace MaestralMauiApp.Pages;

public partial class ImageSelectorPage : ContentPage, INotifyPropertyChanged
{
    public OverlayDrawable OverlayDrawable
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(OverlayDrawable));
            }
        }
    }

    public ImageSelectorPage(ImageSelectorPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        OverlayDrawable = new OverlayDrawable(GraphicOverlay);
        model.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(model.RecognizedTextResult))
            {
                OnRecognizedTextResultChanged(model.RecognizedTextResult ?? new(string.Empty));
            }
        };
        xImage.SizeChanged += Image_SizeChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await OcrPlugin.Default.InitAsync().ConfigureAwait(true);
    }

    private async void Image_SizeChanged(object? sender, EventArgs e)
    {
        if (xImage.Source is FileImageSource fileImageSource)
        {
            Debug.WriteLine(
                $"ImageSelectorPage.Image_SizeChanged. Image source file: {fileImageSource.File}"
            );
            // Expression to open the correct image stream based on file path
            using var stream = Path.IsPathRooted(fileImageSource.File)
                ? File.OpenRead(fileImageSource.File)
                : await FileSystem
                    .OpenAppPackageFileAsync(fileImageSource.File)
                    .ConfigureAwait(true);
            var image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(stream);
            double imageWidth = image.Width;
            double imageHeight = image.Height;

            // Calculate AspectFit displayed size
            var viewWidth = xImage.Width;
            var viewHeight = xImage.Height;
            var scale = Math.Min(viewWidth / imageWidth, viewHeight / imageHeight);
            var displayWidth = imageWidth * scale;
            var displayHeight = imageHeight * scale;

            GraphicOverlay.WidthScaleFactor = scale;
            GraphicOverlay.HeightScaleFactor = scale;
            Debug.WriteLine($"ImageSelectorPage.Image_SizeChanged. Scale: {scale}");
            OnPropertyChanged(nameof(OverlayDrawable));
        }
    }

    void OnRecognizedTextResultChanged(RecognizedTextResult recognizedTextResult)
    {
        Debug.WriteLine(
            $"ImageSelectorPage.{nameof(OnRecognizedTextResultChanged)}. Blocks.Count: {recognizedTextResult.Blocks.Count}"
        );
        OverlayDrawable.Clear();
        foreach (var block in recognizedTextResult.Blocks)
        {
            IDrawable textGraphic = new TextGraphic(
                GraphicOverlay,
                block.Text,
                block.BoundingBox,
                Colors.OrangeRed,
                Colors.White.WithAlpha(0.5f)
            );
            OverlayDrawable.Add(textGraphic);
        }
        OnPropertyChanged(nameof(OverlayDrawable));
        GraphicOverlay.Invalidate();
    }
}
