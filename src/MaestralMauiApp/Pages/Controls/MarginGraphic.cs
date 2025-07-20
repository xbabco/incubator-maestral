// Copyright © 2025 xbabco. All rights reserved.

using System.Diagnostics;

namespace MaestralMauiApp.Pages.Controls;

/// <summary>
/// A graphic to draw text and a bounding box.
/// </summary>
public class MarginGraphic(GraphicsView graphicsView, Color color) : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        Debug.WriteLine($"MarginGraphic.Draw. GraphicsView.Scale: {graphicsView.Scale}");

        var viewBoundingBox = new RectF(
            0,
            0,
            (float)graphicsView.Width,
            (float)graphicsView.Height
        );

        canvas.StrokeColor = color;
        canvas.DrawRectangle(viewBoundingBox);
    }
}
