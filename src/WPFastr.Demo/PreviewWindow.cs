using System.Windows;
using System.Windows.Input;

namespace Olicode.WPFastr.Demo;

internal sealed class PreviewWindow : Window
{
    private readonly BrowserPreview preview;
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
        Width = 1200;
        Height = 720;
        MinWidth = 820;
        MinHeight = 480;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        preview = new BrowserPreview(kind);
        Content = preview;
    }

    protected override void OnClosed(EventArgs e)
    {
        preview.StopLoading();
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
