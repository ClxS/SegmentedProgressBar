namespace SegmentedProgressBar;

using System;
using System.Windows;
using System.Windows.Threading;

public partial class MainWindow : Window
{
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
        "Progress", typeof(double), typeof(MainWindow), new(0.5));

    public static readonly DependencyProperty SmoothProgressProperty = DependencyProperty.Register(
        "SmoothProgress", typeof(double), typeof(MainWindow), new(default(double)));

    private readonly Random random;

    public MainWindow()
    {
        this.InitializeComponent();
        this.random = new();
        var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(1), DispatcherPriority.Normal, this.Callback, Dispatcher.CurrentDispatcher);
    }

    public double SmoothProgress
    {
        get => (double)this.GetValue(SmoothProgressProperty);
        set => this.SetValue(SmoothProgressProperty, value);
    }

    public double Progress
    {
        get => (double)this.GetValue(ProgressProperty);
        set => this.SetValue(ProgressProperty, value);
    }

    private void Callback(object? sender, EventArgs e)
    {
        var newValue = this.Progress + (this.random.NextDouble() - 0.5) / 10.0;
        this.Progress = Math.Clamp(newValue, 0.0, 1.0);

        this.SmoothProgress += 0.001;
        if (this.SmoothProgress > 1.0)
        {
            this.SmoothProgress -= 1.0;
        }
    }
}