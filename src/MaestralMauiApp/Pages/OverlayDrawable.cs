// Copyright © 2025 xbabco. All rights reserved.

using System.Collections.ObjectModel;
using MaestralMauiApp.Pages.Controls;

namespace MaestralMauiApp.Pages;

public class OverlayDrawable : IDrawable
{
    private IList<IDrawable> _defaultDrawables { get; set; }
    private ObservableCollection<IDrawable> _drawables { get; set; }

    public OverlayDrawable(GraphicsView graphicsView)
    {
        _defaultDrawables = [new MarginGraphic(graphicsView, Colors.Violet)];
        _drawables = new ObservableCollection<IDrawable>(_defaultDrawables);
    }

    internal void Clear()
    {
        _drawables.Clear();
        foreach (var drawable in _defaultDrawables)
        {
            _drawables.Add(drawable);
        }
    }

    public void Add(IDrawable drawable)
    {
        _drawables.Add(drawable);
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        foreach (var drawable in _drawables)
        {
            drawable.Draw(canvas, dirtyRect);
        }
        //if (result == null || result.Blocks == null)
        //{
        //    return;
        //}
        //Debug.WriteLine($"OverlayDrawable.Draw. Scale: {scale}");

        //canvas.StrokeColor = Colors.Violet;
        //canvas.StrokeSize = 1;
        //canvas.FontColor = Colors.Violet;
        //canvas.FontSize = 16;

        //canvas.DrawRectangle(0, 0, 200, 200);
        //canvas.DrawRectangle(220, 220, 400, 400);

        //canvas.StrokeColor = Colors.Red;
        //canvas.StrokeSize = 2;
        //canvas.FontColor = Colors.Red;

        //foreach (var block in result.Blocks)
        //{
        //    canvas.FontSize = block.BoundingBox.Height * 0.75f;
        //    canvas.DrawRectangle(
        //        block.BoundingBox.X,
        //        block.BoundingBox.Y,
        //        block.BoundingBox.Width,
        //        block.BoundingBox.Height
        //    );

        //    canvas.DrawString(
        //        block.Text,
        //        block.BoundingBox.X,
        //        block.BoundingBox.Y,
        //        block.BoundingBox.Width,
        //        block.BoundingBox.Height,
        //        HorizontalAlignment.Left,
        //        VerticalAlignment.Top
        //    );
        //}
    }
}
