using System.Windows.Threading;

namespace Olicode.WPFastr.Threading;

public interface IDispatcherService
{
    Task InvokeAsync(Action action, DispatcherPriority priority);
}
