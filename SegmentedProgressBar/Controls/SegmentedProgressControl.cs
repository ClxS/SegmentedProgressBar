namespace SegmentedProgressBar.Controls;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public class SegmentedProgressControl : Control
{
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
        "Progress", typeof(double), typeof(SegmentedProgressControl),
        new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty SegmentDefinitionsProperty = DependencyProperty.Register(
        "SegmentDefinitions", typeof(IEnumerable<Segment>), typeof(SegmentedProgressControl),
        new FrameworkPropertyMetadata(new[]
        {
            new()
            {
                PercentageCutOff = 0.6,
                ActiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x6A, 0xAF, 0x62)),
                InactiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x31, 0x3D, 0x32))
            },
            new Segment
            {
                PercentageCutOff = 0.85,
                ActiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCD, 0xA2, 0x36)),
                InactiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x44, 0x3C, 0x2A))
            },
            new Segment
            {
                PercentageCutOff = 2.0,
                ActiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0x43, 0x54)),
                InactiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x44, 0x28, 0x2E))
            }
        }, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty SegmentCountProperty = DependencyProperty.Register(
        "SegmentCount", typeof(int), typeof(SegmentedProgressControl),
        new FrameworkPropertyMetadata(17, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty SegmentPaddingProperty = DependencyProperty.Register(
        "SegmentPadding", typeof(double), typeof(SegmentedProgressControl),
        new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty BorderRadiusProperty = DependencyProperty.Register(
        "BorderRadius", typeof(double), typeof(SegmentedProgressControl),
        new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public SegmentedProgressControl()
    {
        this.HorizontalAlignment = HorizontalAlignment.Stretch;
        this.Height = 40;
    }

    public double BorderRadius
    {
        get => (double)this.GetValue(BorderRadiusProperty);
        set => this.SetValue(BorderRadiusProperty, value);
    }

    public double SegmentPadding
    {
        get => (double)this.GetValue(SegmentPaddingProperty);
        set => this.SetValue(SegmentPaddingProperty, value);
    }

    public int SegmentCount
    {
        get => (int)this.GetValue(SegmentCountProperty);
        set => this.SetValue(SegmentCountProperty, value);
    }

    public double Progress
    {
        get => (double)this.GetValue(ProgressProperty);
        set => this.SetValue(ProgressProperty, value);
    }

    public IEnumerable<Segment> SegmentDefinitions
    {
        get => (IEnumerable<Segment>)this.GetValue(SegmentDefinitionsProperty);
        set => this.SetValue(SegmentDefinitionsProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        var cellSize = (this.ActualWidth - (this.SegmentCount - 1) * this.SegmentPadding) / this.SegmentCount;

        var x = 0.0;
        for (var i = 0; i < this.SegmentCount; i++)
        {
            var endProgressPercentage = (x + cellSize + this.SegmentPadding) / this.ActualWidth;
            drawingContext.DrawRoundedRectangle(
                this.GetBrushForValue(endProgressPercentage),
                null,
                new(
                    x,
                    0,
                    cellSize,
                    this.ActualHeight),
                this.BorderRadius,
                this.BorderRadius);
            x += cellSize + this.SegmentPadding;
        }
    }

    private Brush GetBrushForValue(double value)
    {
        foreach (var segment in this.SegmentDefinitions)
        {
            if (segment.PercentageCutOff < value)
            {
                continue;
            }

            return this.Progress >= value ? segment.ActiveBrush : segment.InactiveBrush;
        }

        return Brushes.Pink;
    }
}