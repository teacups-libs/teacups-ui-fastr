using System.Windows.Threading;
using Okisioli.Teacups.UI.Core.WPF;
using Okisioli.Teacups.UI.Fastr.WPF.Collections;

namespace Okisioli.Teacups.UI.Fastr.WPF.Demo;

public sealed class BufferedBrowserViewModel : BrowserViewModelBase
{
    public BufferedBrowserViewModel()
    {
        var dispatcherService = new DispatcherService(Dispatcher.CurrentDispatcher);
        CurrentFiles = new DispatcherCollection<FileItem>(dispatcherService);
    }

    public DispatcherCollection<FileItem> CurrentFiles { get; }

    public override void StopLoading()
    {
        base.StopLoading();
        CurrentFiles.BeginUpdate();
    }

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
