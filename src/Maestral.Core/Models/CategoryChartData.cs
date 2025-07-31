// Copyright © 2025 xbabco. All rights reserved.

namespace Maestral.Core.Models;

public class CategoryChartData
{
    public string Title { get; set; } = string.Empty;
    public int Count { get; set; }

    public CategoryChartData(string title, int count)
    {
        Title = title;
        Count = count;
    }
}
