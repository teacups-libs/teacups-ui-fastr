using System.Windows.Threading;

namespace Olivercode.WPFastr.Services;

public interface IDispatcherService
{
    const DispatcherPriority BackgroundPriority = DispatcherPriority.Background;

    Task InvokeAsync(Action action, DispatcherPriority priority);
}
