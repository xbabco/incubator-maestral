// Copyright © 2025 xbabco. All rights reserved.

using System.Text.Json;
using Maestral.Core.Models;
using Microsoft.Extensions.Logging;

namespace Maestral.Core.Data;

public class SeedDataService
{
    private readonly ProjectRepository _projectRepository;
    private readonly TaskRepository _taskRepository;
    private readonly TagRepository _tagRepository;
    private readonly CategoryRepository _categoryRepository;
    private readonly string _seedDataFilePath = "SeedData.json";
    private readonly ILogger<SeedDataService> _logger;

    public SeedDataService(
        ProjectRepository projectRepository,
        TaskRepository taskRepository,
        TagRepository tagRepository,
        CategoryRepository categoryRepository,
        ILogger<SeedDataService> logger
    )
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _tagRepository = tagRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task LoadSeedDataAsync()
    {
        ClearTables();

        var templateStream = await FileSystem
            .OpenAppPackageFileAsync(_seedDataFilePath)
            .ConfigureAwait(false);
        await using var _ = templateStream.ConfigureAwait(false);

        ProjectsJson? payload = null;
        try
        {
            payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.ProjectsJson);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deserializing seed data");
        }

        try
        {
            if (payload is not null)
            {
                foreach (var project in payload.Projects)
                {
                    if (project is null)
                    {
                        continue;
                    }

                    if (project.Category is not null)
                    {
                        await _categoryRepository
                            .SaveItemAsync(project.Category)
                            .ConfigureAwait(false);
                        project.CategoryID = project.Category.ID;
                    }

                    await _projectRepository.SaveItemAsync(project).ConfigureAwait(false);

                    if (project?.Tasks is not null)
                    {
                        foreach (var task in project.Tasks)
                        {
                            task.ProjectID = project.ID;
                            await _taskRepository.SaveItemAsync(task).ConfigureAwait(false);
                        }
                    }

                    if (project?.Tags is not null)
                    {
                        foreach (var tag in project.Tags)
                        {
                            await _tagRepository
                                .SaveItemAsync(tag, project.ID)
                                .ConfigureAwait(false);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error saving seed data");
            throw;
        }
    }

    private async void ClearTables()
    {
        try
        {
            await Task.WhenAll(
                    _projectRepository.DropTableAsync(),
                    _taskRepository.DropTableAsync(),
                    _tagRepository.DropTableAsync(),
                    _categoryRepository.DropTableAsync()
                )
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
