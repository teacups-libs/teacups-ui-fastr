using System.Windows.Threading;
using Olivercode.WPFastr.Threading;

namespace WPFastr.Demo;

public sealed class WpfDispatcherService : IDispatcherService
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
