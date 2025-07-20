// Copyright © 2025 xbabco. All rights reserved.

using System.ComponentModel;

namespace MaestralMauiApp.Pages;

public partial class TextRecognitionImagePage : ContentPage, INotifyPropertyChanged
{
    public IDrawable OverlayDrawable
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

    public TextRecognitionImagePage(TextRecognitionImagePageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        OverlayDrawable = new OverlayDrawable(GraphicOverlay);
    }
}
