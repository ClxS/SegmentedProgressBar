namespace SegmentedProgressBar.Controls;

using System;
using System.Windows.Media;

public class ColourUtility
{
    public struct Hsv
    {
        public double Hue;
        public double Saturation;
        public double Value;
    }
    
    public static Hsv ColorToHsv(Color color)
    {
        int max = Math.Max(color.R, Math.Max(color.G, color.B));
        int min = Math.Min(color.R, Math.Min(color.G, color.B));

        return new()
        {
            Hue = GetHue(color),
            Saturation = max == 0 ? 0 : 1d - 1d * min / max,
            Value = max / 255d
        };
    }

    public static Hsv Lerp(double factor, Hsv left, Hsv right)
    {
        return new()
        {
            Hue = (left.Hue + (right.Hue - left.Hue) * factor),
            Saturation = (left.Saturation + (right.Saturation - left.Saturation) * factor),
            Value = (left.Value + (right.Value - left.Value) * factor),
        };
    }

    public static Color ColorFromHsv(Hsv hsv)
    {
        var hi = (int)Math.Floor(hsv.Hue / 60) % 6;
        var f = hsv.Hue / 60 - Math.Floor(hsv.Hue / 60);

        hsv.Value = hsv.Value * 255;
        var v = (byte)hsv.Value;
        var p = (byte)(hsv.Value * (1 - hsv.Saturation));
        var q = (byte)(hsv.Value * (1 - f * hsv.Saturation));
        var t = (byte)(hsv.Value * (1 - (1 - f) * hsv.Saturation));

        return hi switch
        {
            0 => Color.FromArgb(255, v, t, p),
            1 => Color.FromArgb(255, q, v, p),
            2 => Color.FromArgb(255, p, v, t),
            3 => Color.FromArgb(255, p, q, v),
            4 => Color.FromArgb(255, t, p, v),
            _ => Color.FromArgb(255, v, p, q)
        };
    }
    
    private static void MinMaxRgb(out int min, out int max, int r, int g, int b)
    {
        if (r > g)
        {
            max = r;
            min = g;
        }
        else
        {
            max = g;
            min = r;
        }
        if (b > max)
        {
            max = b;
        }
        else if (b < min)
        {
            min = b;
        }
    }
    
    private static float GetHue(Color color)
    {
        var r = color.R;
        var g = color.G;
        var b = color.B;

        if (r == g && g == b)
            return 0f;

        MinMaxRgb(out var min, out var max, r, g, b);

        float delta = max - min;
        float hue;

        if (r == max)
            hue = (g - b) / delta;
        else if (g == max)
            hue = (b - r) / delta + 2f;
        else
            hue = (r - g) / delta + 4f;

        hue *= 60f;
        if (hue < 0f)
            hue += 360f;

        return hue;
    }
}