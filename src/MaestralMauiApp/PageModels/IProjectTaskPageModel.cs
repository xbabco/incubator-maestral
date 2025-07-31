// Copyright © 2025 xbabco. All rights reserved.

using CommunityToolkit.Mvvm.Input;
using Maestral.Core.Models;

namespace MaestralMauiApp.PageModels;

public interface IProjectTaskPageModel
{
    IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
    bool IsBusy { get; }
}
