using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Olicode.WPFastr.Demo;

internal sealed class PreviewWindow : Window
{
    private readonly BrowserPreview preview;
    private readonly Border breathingBorder;
    private bool escapePressed;

    internal PreviewWindow(PreviewKind kind)
    {
        Title = kind switch
        {
            PreviewKind.Standard => "Standard ListBox — WPFastr Preview",
            PreviewKind.Virtualized => "VirtualizingWrapPanel — WPFastr Preview",
            PreviewKind.Buffered => "BufferedItems — WPFastr Preview",
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
        Width = 1120;
        Height = 720;
        MinWidth = 820;
        MinHeight = 480;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        preview = new BrowserPreview(kind);
        var colorKey = kind switch
        {
            PreviewKind.Standard => "StandardColor",
            PreviewKind.Virtualized => "VirtualizedColor",
            PreviewKind.Buffered => "BufferedColor",
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
        breathingBorder = new Border
        {
            BorderBrush = new SolidColorBrush((Color)Application.Current.FindResource(colorKey)),
            BorderThickness = new Thickness(2),
            Margin = new Thickness(2),
            IsHitTestVisible = false,
            Focusable = false
        };

        // Overlay the border so its animation never changes the browser's available space.
        var host = new Grid();
        host.Children.Add(preview);
        host.Children.Add(breathingBorder);
        Content = host;
        breathingBorder.BeginAnimation(Border.BorderThicknessProperty, new ThicknessAnimation
        {
            From = new Thickness(2),
            To = new Thickness(10),
            Duration = TimeSpan.FromSeconds(1.6),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        });
    }

    protected override void OnClosed(EventArgs e)
    {
        preview.StopLoading();
        breathingBorder.BeginAnimation(Border.BorderThicknessProperty, null);
        base.OnClosed(e);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);
        if (e.Key != Key.Escape)
        {
            escapePressed = false;
            return;
        }

        e.Handled = true;
        if (e.IsRepeat)
            return;

        if (escapePressed)
            Close();
        else
        {
            preview.FocusFolders(selectFirst: true);
            escapePressed = true;
        }
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
        escapePressed = false;
        base.OnPreviewMouseDown(e);
    }
}
