namespace Olicode.WPFastr.Threading;

public enum DispatchPriority
{
    Normal,
    Background,
    Idle
}

public interface IDispatcherService
{
    Task InvokeAsync(Action action, DispatchPriority priority = DispatchPriority.Normal);
}
