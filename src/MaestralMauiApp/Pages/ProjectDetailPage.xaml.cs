// Copyright © 2025 xbabco. All rights reserved.

namespace MaestralMauiApp.Pages;

public partial class ProjectDetailPage : ContentPage
{
    public ProjectDetailPage(ProjectDetailPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
