// Copyright © 2025 xbabco. All rights reserved.

#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maestral.Core.Models;

namespace MaestralMauiApp.PageModels;

public partial class ProjectListPageModel : ObservableObject
{
    private readonly ProjectRepository _projectRepository;

    [ObservableProperty]
    public partial List<Project> Projects { get; set; } = [];

    public ProjectListPageModel(ProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    [RelayCommand]
    private async Task Appearing()
    {
        Projects = await _projectRepository.ListAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    static Task NavigateToProject(Project project) =>
        Shell.Current.GoToAsync($"project?id={project.ID}");

    [RelayCommand]
    static async Task AddProject()
    {
        await Shell.Current.GoToAsync($"project").ConfigureAwait(false);
    }
}
