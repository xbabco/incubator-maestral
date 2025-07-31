// Copyright © 2025 xbabco. All rights reserved.

namespace Maestral.Core.Services;

/// <summary>
/// Platform-independent wrapper for recognized text results, including blocks, lines, and elements.
/// </summary>
public class RecognizedTextResult(string text)
{
    public string Text { get; set; } = text;
    public List<RecognizedTextBlock> Blocks { get; set; } = [];
}

public class RecognizedTextBlock
{
    public string Text { get; set; } = string.Empty;
    public RectF BoundingBox { get; set; } // Position and size of block
}
