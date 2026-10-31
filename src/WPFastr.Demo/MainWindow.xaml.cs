using System.Windows;

namespace Olicode.WPFastr.Demo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenStandardPreview(object sender, RoutedEventArgs e) =>
        ((App)Application.Current).OpenPreview(PreviewKind.Standard);

    private void OpenVirtualizedPreview(object sender, RoutedEventArgs e) =>
        ((App)Application.Current).OpenPreview(PreviewKind.Virtualized);

    private void OpenBufferedPreview(object sender, RoutedEventArgs e) =>
        ((App)Application.Current).OpenPreview(PreviewKind.Buffered);
}
