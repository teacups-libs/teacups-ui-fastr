using System.Windows;

namespace Olicode.WPFastr.Demo;

public partial class App : Application
{
    private bool isShuttingDown;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShowLauncher();
    }

    internal void OpenPreview(PreviewKind kind)
    {
        var launcher = MainWindow;
        var preview = new PreviewWindow(kind);
        preview.Closed += OnPreviewClosed;
        MainWindow = preview;
        preview.Show();
        launcher.Close();
    }

    private void ShowLauncher()
    {
        var launcher = new MainWindow();
        launcher.Closed += OnLauncherClosed;
        MainWindow = launcher;
        launcher.Show();
    }

    private void OnLauncherClosed(object? sender, EventArgs e)
    {
        // Replacing the launcher must not end the application.
        if (ReferenceEquals(MainWindow, sender))
        {
            isShuttingDown = true;
            Shutdown();
        }
    }

    private void OnPreviewClosed(object? sender, EventArgs e)
    {
        if (!isShuttingDown)
            ShowLauncher();
    }

    protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
    {
        isShuttingDown = true;
        base.OnSessionEnding(e);
        if (e.Cancel)
            isShuttingDown = false;
    }
}
