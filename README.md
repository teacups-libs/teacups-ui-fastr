# WPFastr

WPFastr provides cross-platform collection helpers for incrementally replacing large item sets without blocking a UI thread. Framework-specific UI integration lives in separate adapter packages.

Both packages retain the .NET 6 compatibility floor:

- `Olivercode.WPFastr` targets `net6.0` and contains no WPF dependency.
- `Olivercode.WPFastr.Windows` targets `net6.0-windows`, references the core package, and provides WPF adapters and attached behaviors.

## Install

The packages are hosted on GitHub Packages. Add the package source using a GitHub token with `read:packages` permission:

```powershell
dotnet nuget add source "https://nuget.pkg.github.com/oli-g-sk/index.json" `
  --name github `
  --username YOUR_GITHUB_USERNAME `
  --password YOUR_GITHUB_TOKEN `
  --store-password-in-clear-text
```

Install the core package in platform-neutral projects:

```powershell
dotnet add package Olivercode.WPFastr --version VERSION --source github
```

Install the Windows package in WPF projects. It brings in the matching core dependency:

```powershell
dotnet add package Olivercode.WPFastr.Windows --version VERSION --source github
```

## Dispatcher collections

`DispatcherCollection<T>` is available on every .NET 6 platform. The consumer supplies an `IDispatcherService`, allowing each UI framework to map the cross-platform `DispatchPriority` values to its own dispatcher:

```csharp
using Olicode.WPFastr.Collections;
using Olicode.WPFastr.Threading;

var files = new DispatcherCollection<FileItem>(dispatcherService);
var version = files.BeginUpdate();
await files.ReplaceRangeAsync(newFiles, version);
```

The Windows package includes `WpfDispatcherService`:

```csharp
using Olicode.WPFastr.Windows.Threading;

var dispatcherService = new WpfDispatcherService(Application.Current.Dispatcher);
```

Avalonia, MAUI, and other consumers can implement `IDispatcherService` without referencing WPF.

## ItemsControl abstraction

The core package contains `BufferedItemsController`, which owns the collection buffering algorithm. It depends only on `IItemsControlAdapter` and `IDispatcherService`:

```csharp
using Olicode.WPFastr.Controls;

var controller = new BufferedItemsController(itemsControlAdapter, dispatcherService);
controller.SetSource(source);
```

A framework adapter only needs to expose its control's item source:

```csharp
public sealed class FrameworkItemsControlAdapter : IItemsControlAdapter
{
    private readonly FrameworkItemsControl control;

    public IEnumerable? ItemsSource
    {
        get => control.ItemsSource;
        set => control.ItemsSource = value;
    }
}
```

Call `Detach()` when the native control unloads and `SetSource(...)` when it reloads. The Windows package handles this lifecycle automatically for WPF.

## WPF buffered items

Bind `BufferedItems.Source` instead of `ItemsSource`. The behavior works with `ListBox`, `ListView`, or any other WPF `ItemsControl`:

```xml
<Window xmlns:behaviors="clr-namespace:Olicode.WPFastr.Windows.Behaviors;assembly=Olicode.WPFastr.Windows">
    <ListBox behaviors:BufferedItems.Source="{Binding Files}" />
</Window>
```

## WPF scroll reset

Scroll position is preserved by default. Opt in to returning to the top when the source is replaced or reset:

```xml
<ListBox ItemsSource="{Binding Files}"
         behaviors:ItemsControlScroll.ResetOnItemsReset="True" />
```
