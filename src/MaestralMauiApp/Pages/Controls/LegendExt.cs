// Copyright © 2025 xbabco. All rights reserved.

using Syncfusion.Maui.Toolkit.Charts;

namespace MaestralMauiApp.Pages.Controls;

public class LegendExt : ChartLegend
{
    protected override double GetMaximumSizeCoefficient()
    {
        return 0.5;
    }
}
