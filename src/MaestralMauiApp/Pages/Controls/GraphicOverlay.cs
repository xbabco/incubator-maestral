// Copyright © 2025 xbabco. All rights reserved.

using System.Collections.ObjectModel;

namespace MaestralMauiApp.Pages.Controls;

/// <summary>
/// A view which renders a series of custom graphics to be overlayed on top of an associated preview
/// (i.e., the camera preview). The creator can add graphics objects, update the objects, and remove
/// them, triggering the appropriate drawing and invalidation within the view.
///
/// Supports scaling and mirroring of the graphics relative the camera's preview properties. The
/// idea is that detection items are expressed in terms of a preview size, but need to be scaled up
/// to the full view size, and also mirrored in the case of the front-facing camera.
/// </summary>
public partial class GraphicOverlay : GraphicsView
{
    //private readonly Lock _lock = new();

    /// <summary>
    /// Gets the list of graphics to be drawn on the overlay.
    /// </summary>
    public readonly ObservableCollection<IDrawable> Graphics = [];

    /// <summary>
    /// The width of the preview image.
    /// </summary>
    public double PreviewWidth { get; set; }

    /// <summary>
    /// The height of the preview image.
    /// </summary>
    public double PreviewHeight { get; set; }

    /// <summary>
    /// The scale factor for the width.
    /// </summary>
    public double WidthScaleFactor { get; set; } = 1.0f;

    /// <summary>
    /// The scale factor for the height.
    /// </summary>
    public double HeightScaleFactor { get; set; } = 1.0f;

    /// <summary>
    /// Whether to mirror the graphics horizontally.
    /// </summary>
    public bool Mirror { get; set; }

    public GraphicOverlay()
    {
        Graphics.CollectionChanged += (s, e) => Invalidate();
    }

    /// <summary>
    /// Adjusts a horizontal value of the supplied value from the preview scale to the view scale.
    /// </summary>
    public new float ScaleX(float horizontal)
    {
        return (float)(horizontal * WidthScaleFactor);
    }

    /// <summary>
    /// Adjusts a vertical value of the supplied value from the preview scale to the view scale.
    /// </summary>
    public new float ScaleY(float vertical)
    {
        return (float)(vertical * HeightScaleFactor);
    }

    /// <summary>
    /// Adjusts the x coordinate from the preview's coordinate system to the view coordinate system.
    /// </summary>
    public float TranslateX(float x)
    {
        if (Mirror)
        {
            return (float)(Width - ScaleX(x));
        }
        else
        {
            return ScaleX(x);
        }
    }

    /// <summary>
    /// Adjusts the y coordinate from the preview's coordinate system to the view coordinate system.
    /// </summary>
    public float TranslateY(float y)
    {
        return ScaleY(y);
    }
}
