// Copyright © 2025 xbabco. All rights reserved.

using System.Diagnostics;
using Font = Microsoft.Maui.Graphics.Font;

namespace MaestralMauiApp.Pages.Controls;

/// <summary>
/// A graphic to draw text and a bounding box.
/// </summary>
public class TextGraphic(
    GraphicsView graphicsView,
    string text,
    RectF boundingBox,
    Color textColor,
    Color backgroundColor
) : IDrawable
{
    private readonly GraphicOverlay? _overlay = graphicsView as GraphicOverlay;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        Debug.WriteLine(
            $"TextGraphic.Draw. GraphicOverlay.WidthScaleFactor: {_overlay?.WidthScaleFactor} GraphicOverlay.HeightScaleFactor: {_overlay?.HeightScaleFactor}"
        );
        var translatedBoundingBox =
            _overlay != null
                ? new RectF(
                    _overlay.TranslateX(boundingBox.X),
                    _overlay.TranslateY(boundingBox.Y),
                    _overlay.ScaleX(boundingBox.Width),
                    _overlay.ScaleY(boundingBox.Height)
                )
                : new RectF(boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height);

        canvas.FillColor = backgroundColor;
        canvas.FillRectangle(translatedBoundingBox);

        canvas.FontColor = textColor;
        var maxSize = (float)Math.Floor(translatedBoundingBox.Height * 0.8f);
        var fontSize = FindMaxFontSize(canvas, text, translatedBoundingBox, maxSize: maxSize);
        canvas.FontSize = fontSize;
        Debug.WriteLine(
            $"TextGraphic.Draw. text: {text} translatedBoundingBox: {translatedBoundingBox}."
        );
        Debug.WriteLine($"TextGraphic.Draw. canvas.FontSize: {fontSize} maxSize: {maxSize}");
        canvas.DrawString(
            text,
            translatedBoundingBox,
            HorizontalAlignment.Center,
            VerticalAlignment.Center
        );
    }

    static float FindMaxFontSize(
        ICanvas canvas,
        string text,
        RectF rect,
        float minSize = 1f,
        float maxSize = 200f
    )
    {
        var bestSize = minSize;
        while (minSize <= maxSize)
        {
            var mid = (minSize + maxSize) / 2f;
            var size = canvas.GetStringSize(
                text,
                Font.Default,
                mid,
                HorizontalAlignment.Center,
                VerticalAlignment.Center
            );
            if (size.Width <= rect.Width && size.Height <= rect.Height)
            {
                bestSize = mid;
                minSize = mid + 0.5f; // Try larger
            }
            else
            {
                maxSize = mid - 0.5f; // Try smaller
            }
        }
        return bestSize / 1.10f;
    }
}
