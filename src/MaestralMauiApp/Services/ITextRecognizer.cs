// Copyright © 2025 xbabco. All rights reserved.

namespace MaestralMauiApp.Services;

public interface ITextRecognizer
{
    Task<RecognizedTextResult> RecognizeTextAsync(Stream imageStream);
}
