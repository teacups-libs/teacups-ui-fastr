using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Olicode.WPFastr.Demo;

public abstract class BrowserViewModelBase : INotifyPropertyChanged
{
    private string currentFolder = "Select a folder";
    private string? selectedFolder;
    private string statusText = "0 files";
    private long currentUpdateVersion;

    protected BrowserViewModelBase()
    {
        for (var i = 1; i <= 100; i++)
            Folders.Add($"Folder {i}");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<string> Folders { get; } = new();

    public string CurrentFolder
    {
        get => currentFolder;
        private set => SetField(ref currentFolder, value);
    }

    public string? SelectedFolder
    {
        get => selectedFolder;
        set
        {
            if (!SetField(ref selectedFolder, value) || value is null)
                return;

            _ = LoadFolderAsync(value);
        }
    }

    public string StatusText
    {
        get => statusText;
        private set => SetField(ref statusText, value);
    }

    protected abstract Task ReplaceFilesAsync(IReadOnlyList<FileItem> files, long updateVersion);

    protected abstract bool IsCurrent(long updateVersion);

    private async Task LoadFolderAsync(string folder)
    {
        var updateVersion = Interlocked.Increment(ref currentUpdateVersion);

        CurrentFolder = folder;
        StatusText = "Loading...";

        var files = await Task.Run(() => DemoFileGenerator.GenerateFiles(folder));

        await ReplaceFilesAsync(files, updateVersion);

        if (IsCurrent(updateVersion))
            StatusText = $"{files.Count} files";
    }

    protected bool IsLatest(long updateVersion)
    {
        return Volatile.Read(ref currentUpdateVersion) == updateVersion;
    }

    protected void SetStatus(string value)
    {
        StatusText = value;
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
