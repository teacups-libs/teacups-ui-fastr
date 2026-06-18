using System.Windows.Threading;

namespace Olivercode.WPFastr.Threading;

public interface IDispatcherService
{
    const DispatcherPriority BackgroundPriority = DispatcherPriority.Background;

    Task InvokeAsync(Action action, DispatcherPriority priority);
}
