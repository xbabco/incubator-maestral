namespace MaestralMauiApp.Services;

public interface ITextRecognizer
{
    Task<RecognizedTextResult> RecognizeTextAsync(Stream imageStream);
}
