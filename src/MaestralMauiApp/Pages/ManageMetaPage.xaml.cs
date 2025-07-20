// Copyright © 2025 xbabco. All rights reserved.

namespace MaestralMauiApp.Pages;

public partial class ManageMetaPage : ContentPage
{
    public ManageMetaPage(ManageMetaPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
