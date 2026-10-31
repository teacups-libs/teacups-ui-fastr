using System.Windows;

namespace Olicode.WPFastr.Demo;

internal sealed class PreviewWindow : Window
{
    private readonly BrowserPreview preview;

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
        Content = preview;
    }

    protected override void OnClosed(EventArgs e)
    {
        preview.StopLoading();
        base.OnClosed(e);
    }
}
