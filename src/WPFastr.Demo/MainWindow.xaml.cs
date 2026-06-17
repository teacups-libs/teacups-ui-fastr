using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using Olivercode.WPFastr;
using Olivercode.WPFastr.Services;

namespace WPFastr.Demo;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private static readonly string[] Extensions =
    {
        "txt",
        "pdf",
        "docx",
        "xlsx",
        "png",
        "jpg",
        "cs",
        "json",
        "zip"
    };

    private readonly string[] nameParts =
    {
        "Budget",
        "Meeting notes",
        "Invoice",
        "Screenshot",
        "Spec",
        "Draft",
        "Report",
        "Archive",
        "Source",
        "Reference",
        "Export",
        "Plan"
    };

    private string currentFolder = "Select a folder";
    private string statusText = "0 files";

    public MainWindow()
    {
        CurrentFiles = new DispatcherCollection<FileItem>(new WpfDispatcherService(Dispatcher));

        InitializeComponent();

        for (var i = 1; i <= 100; i++)
            Folders.Add($"Folder {i}");

        DataContext = this;
        FolderList.SelectedIndex = 0;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<string> Folders { get; } = new();

    public DispatcherCollection<FileItem> CurrentFiles { get; }

    public string CurrentFolder
    {
        get => currentFolder;
        private set => SetField(ref currentFolder, value);
    }

    public string StatusText
    {
        get => statusText;
        private set => SetField(ref statusText, value);
    }

    private async void FolderList_OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (FolderList.SelectedItem is not string folder)
            return;

        CurrentFolder = folder;
        StatusText = "Loading...";

        var updateVersion = CurrentFiles.BeginUpdate();
        var files = await Task.Run(() => GenerateFiles(folder));

        await CurrentFiles.ReplaceRangeAsync(files, updateVersion);

        if (CurrentFiles.IsCurrent(updateVersion))
            StatusText = $"{files.Count} files";
    }

    private List<FileItem> GenerateFiles(string folder)
    {
        var fileCount = Random.Shared.Next(10, 501);
        var files = new List<FileItem>(fileCount);
        var folderNumber = folder.Replace("Folder ", string.Empty, StringComparison.Ordinal);

        for (var i = 1; i <= fileCount; i++)
        {
            var extension = Extensions[Random.Shared.Next(Extensions.Length)];
            var namePart = nameParts[Random.Shared.Next(nameParts.Length)];
            var size = Random.Shared.Next(12, 50_000);
            var modified = DateTime.Today.AddDays(-Random.Shared.Next(0, 365));

            files.Add(new FileItem(
                $"{namePart} {folderNumber}-{i:000}.{extension}",
                extension.ToUpperInvariant(),
                FormatSize(size),
                modified.ToString("MMM d")));
        }

        return files;
    }

    private static string FormatSize(int kilobytes)
    {
        return kilobytes < 1024
            ? $"{kilobytes} KB"
            : $"{kilobytes / 1024.0:0.#} MB";
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public sealed record FileItem(string Name, string Extension, string SizeText, string ModifiedText);

    private sealed class WpfDispatcherService : IDispatcherService
    {
        private readonly Dispatcher dispatcher;

        public WpfDispatcherService(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public Task InvokeAsync(Action action, DispatcherPriority priority)
        {
            return dispatcher.InvokeAsync(action, priority).Task;
        }
    }
}
