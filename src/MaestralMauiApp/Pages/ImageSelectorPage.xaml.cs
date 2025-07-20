// Copyright © 2025 xbabco. All rights reserved.

namespace MaestralMauiApp.Pages;

public partial class ImageSelectorPage : ContentPage
{
    public ImageSelectorPage(ImageSelectorPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
