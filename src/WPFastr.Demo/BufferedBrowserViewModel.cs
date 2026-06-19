using System.Windows.Threading;
using Olicode.WPFastr.Collections;
using Olicode.WPFastr.Windows.Threading;

namespace Olicode.WPFastr.Demo;

public sealed class BufferedBrowserViewModel : BrowserViewModelBase
{
    public BufferedBrowserViewModel(Dispatcher dispatcher)
    {
        CurrentFiles = new DispatcherCollection<FileItem>(new WpfDispatcherService(dispatcher));
    }

    public DispatcherCollection<FileItem> CurrentFiles { get; }

    protected override async Task ReplaceFilesAsync(IReadOnlyList<FileItem> files, long updateVersion)
    {
        if (!IsLatest(updateVersion))
            return;

        var collectionVersion = CurrentFiles.BeginUpdate();

        if (!IsLatest(updateVersion))
            return;

        await CurrentFiles.ReplaceRangeAsync(files, collectionVersion);
    }

    protected override bool IsCurrent(long updateVersion)
    {
        return IsLatest(updateVersion);
    }
}
