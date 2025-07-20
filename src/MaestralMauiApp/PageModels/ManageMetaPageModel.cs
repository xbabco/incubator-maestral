// Copyright © 2025 xbabco. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaestralMauiApp.Models;

namespace MaestralMauiApp.PageModels;

public partial class ManageMetaPageModel : ObservableObject
{
    private readonly CategoryRepository _categoryRepository;
    private readonly TagRepository _tagRepository;
    private readonly SeedDataService _seedDataService;

    [ObservableProperty]
    public partial ObservableCollection<Category> Categories { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<Tag> Tags { get; set; } = [];

    public ManageMetaPageModel(
        CategoryRepository categoryRepository,
        TagRepository tagRepository,
        SeedDataService seedDataService
    )
    {
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _seedDataService = seedDataService;
    }

    private async Task LoadData()
    {
        var categoriesList = await _categoryRepository.ListAsync().ConfigureAwait(false);
        Categories = new ObservableCollection<Category>(categoriesList);
        var tagsList = await _tagRepository.ListAsync().ConfigureAwait(false);
        Tags = new ObservableCollection<Tag>(tagsList);
    }

    [RelayCommand]
    private Task Appearing() => LoadData();

    [RelayCommand]
    private async Task SaveCategories()
    {
        foreach (var category in Categories)
        {
            await _categoryRepository.SaveItemAsync(category).ConfigureAwait(false);
        }

        await AppShell.DisplayToastAsync("Categories saved").ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task DeleteCategory(Category category)
    {
        Categories.Remove(category);
        await _categoryRepository.DeleteItemAsync(category).ConfigureAwait(false);
        await AppShell.DisplayToastAsync("Category deleted").ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task AddCategory()
    {
        var category = new Category();
        Categories.Add(category);
        await _categoryRepository.SaveItemAsync(category).ConfigureAwait(false);
        await AppShell.DisplayToastAsync("Category added").ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task SaveTags()
    {
        foreach (var tag in Tags)
        {
            await _tagRepository.SaveItemAsync(tag).ConfigureAwait(false);
        }

        await AppShell.DisplayToastAsync("Tags saved").ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task DeleteTag(Tag tag)
    {
        Tags.Remove(tag);
        await _tagRepository.DeleteItemAsync(tag).ConfigureAwait(false);
        await AppShell.DisplayToastAsync("Tag deleted").ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task AddTag()
    {
        var tag = new Tag();
        Tags.Add(tag);
        await _tagRepository.SaveItemAsync(tag).ConfigureAwait(false);
        await AppShell.DisplayToastAsync("Tag added").ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task Reset()
    {
        Preferences.Default.Remove("is_seeded");
        await _seedDataService.LoadSeedDataAsync().ConfigureAwait(false);
        Preferences.Default.Set("is_seeded", true);
        await Shell.Current.GoToAsync("//main").ConfigureAwait(false);
    }
}
