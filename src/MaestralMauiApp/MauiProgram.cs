// Copyright © 2025 xbabco. All rights reserved.

using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.Maui.OCR;
using Syncfusion.Maui.Toolkit.Hosting;

namespace MaestralMauiApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit()
            .ConfigureMauiHandlers(handlers =>
            {
#if IOS || MACCATALYST
                handlers.AddHandler<
                    Microsoft.Maui.Controls.CollectionView,
                    Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2
                >();
#endif
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
            });

#if DEBUG
        builder.Logging.AddDebug();
        builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        builder.Services.AddSingleton<ProjectRepository>();
        builder.Services.AddSingleton<TaskRepository>();
        builder.Services.AddSingleton<CategoryRepository>();
        builder.Services.AddSingleton<TagRepository>();
        builder.Services.AddSingleton<SeedDataService>();
        builder.Services.AddSingleton<ModalErrorHandler>();
        builder.Services.AddSingleton<MainPageModel>();
        builder.Services.AddSingleton<ProjectListPageModel>();
        builder.Services.AddSingleton<ManageMetaPageModel>();

        builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>(
            "project"
        );
        builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");
        builder.Services.AddTransientWithShellRoute<ImageSelectorPage, ImageSelectorPageModel>(
            "imageselector"
        );
        builder.Services.AddTransientWithShellRoute<
            TextRecognitionImagePage,
            TextRecognitionImagePageModel
        >("textrecognitionimage");

        builder.Services.AddSingleton(OcrPlugin.Default);

        return builder.Build();
    }
}

public class SomeViewModel(ITextRecognizer textRecognizer)
{
    public async Task<RecognizedTextResult> Recognize(Stream imageStream)
    {
        var text = await textRecognizer.RecognizeTextAsync(imageStream).ConfigureAwait(false);
        return text;
    }
}
