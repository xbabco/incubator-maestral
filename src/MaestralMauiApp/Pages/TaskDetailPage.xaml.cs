// Copyright © 2025 xbabco. All rights reserved.

namespace MaestralMauiApp.Pages;

public partial class TaskDetailPage : ContentPage
{
    public TaskDetailPage(TaskDetailPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
