using System.Windows;

namespace Olicode.WPFastr.Demo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        BufferedBrowser = new BufferedBrowserViewModel(Dispatcher);
        StandardBrowser = new StandardBrowserViewModel();
        SecondStandardBrowser = new StandardBrowserViewModel();
        VirtualizedBrowser = new StandardBrowserViewModel();
        SecondVirtualizedBrowser = new StandardBrowserViewModel();

        InitializeComponent();
        DataContext = this;

        BufferedBrowser.SelectedFolder = BufferedBrowser.Folders[0];
        StandardBrowser.SelectedFolder = StandardBrowser.Folders[0];
        SecondStandardBrowser.SelectedFolder = SecondStandardBrowser.Folders[0];
        VirtualizedBrowser.SelectedFolder = VirtualizedBrowser.Folders[0];
        SecondVirtualizedBrowser.SelectedFolder = SecondVirtualizedBrowser.Folders[0];
    }

    public BufferedBrowserViewModel BufferedBrowser { get; }

    public StandardBrowserViewModel StandardBrowser { get; }

    public StandardBrowserViewModel SecondStandardBrowser { get; }

    public StandardBrowserViewModel VirtualizedBrowser { get; }

    public StandardBrowserViewModel SecondVirtualizedBrowser { get; }
}
