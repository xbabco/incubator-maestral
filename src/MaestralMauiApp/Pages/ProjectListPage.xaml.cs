// Copyright © 2025 xbabco. All rights reserved.

namespace MaestralMauiApp.Pages;

public partial class ProjectListPage : ContentPage
{
    public ProjectListPage(ProjectListPageModel model)
    {
        BindingContext = model;
        InitializeComponent();
    }
}
