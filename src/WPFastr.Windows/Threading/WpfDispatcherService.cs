using System.Windows.Threading;
using Olicode.WPFastr.Threading;

namespace Olicode.WPFastr.Windows.Threading;

public sealed class WpfDispatcherService : IDispatcherService
{
    private readonly Dispatcher dispatcher;

    public WpfDispatcherService(Dispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    public Task InvokeAsync(Action action, DispatchPriority priority = DispatchPriority.Normal)
    {
        return dispatcher.InvokeAsync(action, MapPriority(priority)).Task;
    }

    private static DispatcherPriority MapPriority(DispatchPriority priority)
    {
        return priority switch
        {
            DispatchPriority.Background => DispatcherPriority.Background,
            DispatchPriority.Idle => DispatcherPriority.ApplicationIdle,
            _ => DispatcherPriority.Normal
        };
    }
}
