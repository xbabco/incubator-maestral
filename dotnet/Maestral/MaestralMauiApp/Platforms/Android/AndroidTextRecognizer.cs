using Android.Gms.Tasks;
using Android.Graphics;
using Xamarin.Google.MLKit.Vision.Common;
using Xamarin.Google.MLKit.Vision.Text;
using Xamarin.Google.MLKit.Vision.Text.Latin;

namespace MaestralMauiApp;

public class AndroidTextRecognizer : Services.ITextRecognizer
{
    public async Task<RecognizedTextResult> RecognizeTextAsync(Stream imageStream)
    {
        ArgumentNullException.ThrowIfNull(imageStream, nameof(imageStream));

        Bitmap? bitmap = BitmapFactory.DecodeStream(imageStream);
        if (bitmap == null)
        {
            throw new InvalidOperationException("Failed to convert Stream to Bitmap.");
        }

        var recognizer = TextRecognition.GetClient(TextRecognizerOptions.DefaultOptions);
        var inputImage = InputImage.FromBitmap(bitmap, 0);

        var tcs = new TaskCompletionSource<RecognizedTextResult>();
        recognizer.Process(inputImage)
            .AddOnSuccessListener(new OnSuccessListener(result =>
            {
                var recognized = new RecognizedTextResult(result.GetText());
                foreach (var block in result.TextBlocks)
                {
                    var blockModel = new RecognizedTextBlock { Text = block.Text };
                    foreach (var line in block.Lines)
                    {
                        var lineModel = new RecognizedTextLine { Text = line.Text };
                        foreach (var element in line.Elements)
                        {
                            var elementModel = new RecognizedTextElement { Text = element.Text };
                            lineModel.Elements.Add(elementModel);
                        }
                        blockModel.Lines.Add(lineModel);
                    }
                    recognized.Blocks.Add(blockModel);
                }
                tcs.SetResult(recognized);
            }))
            .AddOnFailureListener(new OnFailureListener(ex =>
            {
                tcs.SetException(ex);
            }));

        return await tcs.Task;
    }

    public async Task<string> RecognizeTextAsync(ImageSource imageSource)
    {
        ArgumentNullException.ThrowIfNull(imageSource, nameof(imageSource));

        Bitmap? bitmap = null;
        if (imageSource is FileImageSource fileImageSource)
        {
            // Load bitmap from file path
            var filePath = fileImageSource.File;
            bitmap = BitmapFactory.DecodeFile(filePath);
        }
        else if (imageSource is StreamImageSource streamImageSource)
        {
            // Load bitmap from stream
            var stream = await streamImageSource.Stream(System.Threading.CancellationToken.None);
            bitmap = BitmapFactory.DecodeStream(stream);
        }
        // Add other ImageSource types as needed

        if (bitmap == null)
        {
            throw new InvalidOperationException("Failed to convert ImageSource to Bitmap.");
        }

        var recognizer = TextRecognition.GetClient(TextRecognizerOptions.DefaultOptions);
        var inputImage = InputImage.FromBitmap(bitmap, 0);

        var tcs = new System.Threading.Tasks.TaskCompletionSource<string>();
        recognizer.Process(inputImage)
            .AddOnSuccessListener(new OnSuccessListener(result =>
            {
                var txt = result.GetText();
                tcs.SetResult(txt);
            }))
            .AddOnFailureListener(new OnFailureListener(ex =>
            {
                tcs.SetException(ex);
            }));

        return await tcs.Task;
    }

    public static async Task<string> RecognizeTextAsync0(string imagePath)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(imagePath, nameof(imagePath));

        var res = global::Android.App.Application.Context.Resources ?? throw new InvalidOperationException("Android resources are not available.");
        var imageNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(imagePath);
        int id = res.GetIdentifier(imageNameWithoutExtension, "drawable", global::Android.App.Application.Context.PackageName);
        if (id == 0)
        {
            throw new ArgumentException($"Resource with name '{imagePath}' not found.", nameof(imagePath));
        }

        Bitmap? bitmap = BitmapFactory.DecodeResource(res, id)
            ?? throw new InvalidOperationException($"Failed to decode resource '{imagePath}' to Bitmap.");
        var recognizer = TextRecognition.GetClient(TextRecognizerOptions.DefaultOptions);
        var inputImage = InputImage.FromBitmap(bitmap, 0);

        var tcs = new TaskCompletionSource<string>();
        recognizer.Process(inputImage)
            .AddOnSuccessListener(new OnSuccessListener(result =>
            {
                var txt = result.GetText();
                tcs.SetResult(txt);
            }))
            .AddOnFailureListener(new OnFailureListener(ex =>
            {
                tcs.SetException(ex);
            }));

        return await tcs.Task;
    }
}

internal class OnFailureListener(Action<Java.Lang.Exception> onFailure)
    : Java.Lang.Object, IOnFailureListener
{
    private readonly Action<Java.Lang.Exception> _onFailure = onFailure ?? throw new ArgumentNullException(nameof(onFailure));

    public void OnFailure(Java.Lang.Exception e)
    {
        _onFailure(e);
    }
}

internal class OnSuccessListener(Action<Xamarin.Google.MLKit.Vision.Text.Text> onSuccess)
    : Java.Lang.Object, IOnSuccessListener
{
    private readonly Action<Xamarin.Google.MLKit.Vision.Text.Text> _onSuccess = onSuccess ?? throw new ArgumentNullException(nameof(onSuccess));

    public void OnSuccess(Java.Lang.Object? result)
    {
        if (result is Xamarin.Google.MLKit.Vision.Text.Text text)
        {
            _onSuccess(text);
        }
    }
}
