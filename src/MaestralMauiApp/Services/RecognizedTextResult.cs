// Copyright © 2025 xbabco. All rights reserved.

namespace MaestralMauiApp.Services;

/// <summary>
/// Platform-independent wrapper for recognized text results, including blocks, lines, and elements.
/// </summary>
public class RecognizedTextResult
{
    public string Text { get; set; } = string.Empty;
    public List<RecognizedTextBlock> Blocks { get; set; } = [];

    public RecognizedTextResult(string text)
    {
        Text = text;
        Blocks = new List<RecognizedTextBlock>();
    }
}

public class RecognizedTextBlock
{
    public string Text { get; set; } = string.Empty;
    public List<RecognizedTextLine> Lines { get; set; } = new();
}

public class RecognizedTextLine
{
    public string Text { get; set; } = string.Empty;
    public List<RecognizedTextElement> Elements { get; set; } = new();
}

public class RecognizedTextElement
{
    public string Text { get; set; } = string.Empty;
}
