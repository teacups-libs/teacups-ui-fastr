# WPFastr

WPFastr provides small WPF helpers for displaying collections without blocking the UI while large item sets are replaced.

It targets .NET 6 for Windows and can attach to any WPF `ItemsControl`.

## Install

The package is hosted on GitHub Packages. Add the package source using a GitHub token with `read:packages` permission:

```powershell
dotnet nuget add source "https://nuget.pkg.github.com/oli-g-sk/index.json" `
  --name github `
  --username YOUR_GITHUB_USERNAME `
  --password YOUR_GITHUB_TOKEN `
  --store-password-in-clear-text

dotnet add package Olivercode.WPFastr --version VERSION --source github
```

## Use buffered items

Bind `BufferedItems.Source` instead of `ItemsSource`. The behavior works with `ListBox`, `ListView`, or any other `ItemsControl`:

```xml
<Window xmlns:behaviors="clr-namespace:Olivercode.WPFastr.Behaviors;assembly=Olivercode.WPFastr">
    <ListBox behaviors:BufferedItems.Source="{Binding Files}" />
</Window>
```

Use `DispatcherCollection<T>` when replacing a collection incrementally:

```csharp
using Olivercode.WPFastr.Collections;
using Olivercode.WPFastr.Threading;
using System.Windows.Threading;

public sealed class WpfDispatcherService : IDispatcherService
{
    private readonly Dispatcher dispatcher;

    public WpfDispatcherService(Dispatcher dispatcher) => this.dispatcher = dispatcher;

    public Task InvokeAsync(Action action, DispatcherPriority priority) =>
        dispatcher.InvokeAsync(action, priority).Task;
}

var files = new DispatcherCollection<FileItem>(
    new WpfDispatcherService(Application.Current.Dispatcher));

var version = files.BeginUpdate();
await files.ReplaceRangeAsync(newFiles, version);
```

## Reset scrolling

Scroll position is preserved by default. Opt in to returning to the top when the source is replaced or reset:

```xml
<ListBox ItemsSource="{Binding Files}"
         behaviors:ItemsControlScroll.ResetOnItemsReset="True" />
```
