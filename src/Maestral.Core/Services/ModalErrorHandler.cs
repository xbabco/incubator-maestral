// Copyright © 2025 xbabco. All rights reserved.

namespace Maestral.Core.Services;

/// <summary>
/// Modal Error Handler.
/// </summary>
public class ModalErrorHandler : IErrorHandler
{
    readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Handle error in UI.
    /// </summary>
    /// <param name="ex">Exception.</param>
    public void HandleError(Exception ex)
    {
        DisplayAlert(ex).FireAndForgetSafeAsync();
    }

    async Task DisplayAlert(Exception ex)
    {
        try
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            if (Shell.Current is Shell shell)
            {
                await shell.DisplayAlert("Error", ex.Message, "OK").ConfigureAwait(false);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
