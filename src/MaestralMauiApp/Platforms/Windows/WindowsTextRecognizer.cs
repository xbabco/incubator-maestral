// Copyright © 2025 xbabco. All rights reserved.

public class WindowsTextRecognizer : MaestralMauiApp.Services.ITextRecognizer
{
    public Task<RecognizedTextResult> RecognizeTextAsync(Stream imageStream)
    {
        // Implement Windows OCR logic here, or return a stub for now
        return Task.FromResult(
            new RecognizedTextResult("Text recognition not implemented for Windows.")
        );
    }
}
