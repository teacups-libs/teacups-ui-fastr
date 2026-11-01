using System.Windows.Threading;

namespace Okisioli.Teacups.UI.Fastr.WPF.Threading;

public interface IDispatcherService
{
    Task InvokeAsync(Action action, DispatcherPriority priority);
}
