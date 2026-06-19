using System.Collections;

namespace Olicode.WPFastr.Controls;

public interface IItemsControlAdapter
{
    IEnumerable? ItemsSource { get; set; }
}
