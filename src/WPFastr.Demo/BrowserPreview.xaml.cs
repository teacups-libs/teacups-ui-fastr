using System.Windows;
using System.Windows.Controls;

namespace Olicode.WPFastr.Demo;

public partial class BrowserPreview : UserControl
{
    private readonly BrowserViewModelBase browser;

    internal BrowserPreview(PreviewKind kind)
    {
        browser = kind switch
        {
            PreviewKind.Standard or PreviewKind.Virtualized => new StandardBrowserViewModel(),
            PreviewKind.Buffered => new BufferedBrowserViewModel(Dispatcher),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        InitializeComponent();
        DataContext = browser;

        var templateKey = kind switch
        {
            PreviewKind.Standard => "StandardBrowserTemplate",
            PreviewKind.Virtualized => "VirtualizedBrowserTemplate",
            PreviewKind.Buffered => "BufferedBrowserTemplate",
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        BrowserContent.ContentTemplate = (DataTemplate)Resources[templateKey];
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        browser.SelectedFolder = browser.Folders[0];
    }

    internal void StopLoading() => browser.StopLoading();
}
