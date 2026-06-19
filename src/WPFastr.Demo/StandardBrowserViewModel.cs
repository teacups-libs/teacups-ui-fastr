using System.Collections.ObjectModel;

namespace Olicode.WPFastr.Demo;

public sealed class StandardBrowserViewModel : BrowserViewModelBase
{
    public ObservableCollection<FileItem> CurrentFiles { get; } = new();

    protected override Task ReplaceFilesAsync(IReadOnlyList<FileItem> files, long updateVersion)
    {
        if (!IsLatest(updateVersion))
            return Task.CompletedTask;

        CurrentFiles.Clear();

        foreach (var file in files)
            CurrentFiles.Add(file);

        return Task.CompletedTask;
    }

    protected override bool IsCurrent(long updateVersion)
    {
        return IsLatest(updateVersion);
    }
}
