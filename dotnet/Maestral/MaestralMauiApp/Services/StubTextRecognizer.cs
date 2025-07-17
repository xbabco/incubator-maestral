namespace MaestralMauiApp.Services;

public class StubTextRecognizer: ITextRecognizer
{
    public Task<RecognizedTextResult> RecognizeTextAsync(Stream imageStream)
    {
        throw new NotImplementedException(
            "Text recognition is not implemented in the stub recognizer. Please implement this method in a platform-specific recognizer.");
    }
}