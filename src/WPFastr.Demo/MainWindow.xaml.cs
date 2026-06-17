using System.Windows;

namespace WPFastr.Demo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        BufferedBrowser = new BufferedBrowserViewModel(Dispatcher);
        StandardBrowser = new StandardBrowserViewModel();

        InitializeComponent();
        DataContext = this;

        BufferedBrowser.SelectedFolder = BufferedBrowser.Folders[0];
        StandardBrowser.SelectedFolder = StandardBrowser.Folders[0];
    }

    public BufferedBrowserViewModel BufferedBrowser { get; }

    public StandardBrowserViewModel StandardBrowser { get; }
}
