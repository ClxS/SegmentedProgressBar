namespace SegmentedProgressBar.Controls;

using System.Windows.Media;

public class Segment
{
    public double PercentageCutOff { get; set; }

    public Brush ActiveBrush { get; set; }

    public Brush InactiveBrush { get; set; }
}