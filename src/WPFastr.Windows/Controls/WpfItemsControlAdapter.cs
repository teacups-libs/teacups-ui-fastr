using System.Collections;
using System.Windows.Controls;
using Olicode.WPFastr.Controls;

namespace Olicode.WPFastr.Windows.Controls;

public sealed class WpfItemsControlAdapter : IItemsControlAdapter
{
    private readonly ItemsControl itemsControl;

    public WpfItemsControlAdapter(ItemsControl itemsControl)
    {
        this.itemsControl = itemsControl;
    }

    public IEnumerable? ItemsSource
    {
        get => itemsControl.ItemsSource;
        set => itemsControl.ItemsSource = value;
    }
}
