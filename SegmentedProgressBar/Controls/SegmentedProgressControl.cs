namespace SegmentedProgressBar.Controls;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

    public static readonly DependencyProperty ShowScalarValueProperty = DependencyProperty.Register(
        "ShowScalarValue", typeof(bool), typeof(SegmentedProgressControl),
        new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ScalarFontProperty = DependencyProperty.Register(
        "ScalarFont", typeof(Typeface), typeof(SegmentedProgressControl), new FrameworkPropertyMetadata(
            new Typeface(new("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
            FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ReserveScalarAreaWhenNonScalarProperty = DependencyProperty.Register(
        "ReserveScalarAreaWhenNonScalar", typeof(bool), typeof(SegmentedProgressControl),
        new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty SmoothEndInterpolationProperty = DependencyProperty.Register(
        "SmoothEndInterpolation", typeof(bool), typeof(SegmentedProgressControl), new(default(bool)));

    public SegmentedProgressControl()
    {
        this.HorizontalAlignment = HorizontalAlignment.Stretch;
        this.Height = 40;
    }

    public bool SmoothEndInterpolation
    {
        get => (bool)this.GetValue(SmoothEndInterpolationProperty);
        set => this.SetValue(SmoothEndInterpolationProperty, value);
    }

    public bool ReserveScalarAreaWhenNonScalar
    {
        get => (bool)this.GetValue(ReserveScalarAreaWhenNonScalarProperty);
        set => this.SetValue(ReserveScalarAreaWhenNonScalarProperty, value);
    }

    public Typeface ScalarFont
    {
        get => (Typeface)this.GetValue(ScalarFontProperty);
        set => this.SetValue(ScalarFontProperty, value);
    }

    public bool ShowScalarValue
    {
        get => (bool)this.GetValue(ShowScalarValueProperty);
        set => this.SetValue(ShowScalarValueProperty, value);
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
        var scalarText = this.ShowScalarValue ? this.FormattedText() : null;
        var barWidth = this.ShowScalarValue || this.ReserveScalarAreaWhenNonScalar ? this.ActualWidth - 50.0 : this.ActualWidth;

        var cellSize = (barWidth - (this.SegmentCount - 1) * this.SegmentPadding) / this.SegmentCount;

        var x = 0.0;
        for (var i = 0; i < this.SegmentCount; i++)
        {
            var startProgressPercentage = x / barWidth;
            var endProgressPercentage = (x + cellSize) / barWidth;

            Brush cellBrush;
            if (this.SmoothEndInterpolation && startProgressPercentage <= this.Progress && endProgressPercentage >= this.Progress)
            {
                cellBrush = this.GetInterpolatedBrush(startProgressPercentage, endProgressPercentage);
            }
            else
            {
                cellBrush = this.GetBrushForValue(endProgressPercentage);
            }

            drawingContext.DrawRoundedRectangle(
                cellBrush,
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

        if (this.ShowScalarValue)
        {
            drawingContext.DrawText(scalarText, new(this.ActualWidth - scalarText.Width, this.ActualHeight / 2.0 - scalarText.Height / 2.0));
        }
    }

    private Brush GetInterpolatedBrush(double startValue, double endValue)
    {
        var percentage = (this.Progress - startValue) / (endValue - startValue);
        var segment = this.GetSegment(this.Progress);
        if (segment is not { ActiveBrush: SolidColorBrush, InactiveBrush: SolidColorBrush })
        {
            return Brushes.Pink;
        }

        var startColourHsv = ColourUtility.ColorToHsv(((SolidColorBrush)segment.InactiveBrush).Color);
        var endColourHsv = ColourUtility.ColorToHsv(((SolidColorBrush)segment.ActiveBrush).Color);
        var finalColourHsv = ColourUtility.Lerp(percentage, startColourHsv, endColourHsv);
        var finalColourRgb = ColourUtility.ColorFromHsv(finalColourHsv);
        return new SolidColorBrush(finalColourRgb);
    }

    private FormattedText FormattedText()
    {
        return new(
            $"{(int)(this.Progress * 100)}%",
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            this.ScalarFont,
            16,
            this.GetBrushForValue(this.Progress),
            new(),
            TextFormattingMode.Display,
            1);
    }

    private Brush GetBrushForValue(double value)
    {
        var segment = this.GetSegment(value);
        if (segment != null)
        {
            return this.Progress >= value ? segment.ActiveBrush : segment.InactiveBrush;
        }

        return Brushes.Pink;
    }

    private Segment? GetSegment(double value)
    {
        return this.SegmentDefinitions.FirstOrDefault(segment => !(segment.PercentageCutOff < value));
    }
}