using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Olicode.WPFastr.Controls;
using Olicode.WPFastr.Windows.Controls;
using Olicode.WPFastr.Windows.Threading;

namespace Olicode.WPFastr.Windows.Behaviors;

public static class BufferedItems
{
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.RegisterAttached(
            "Source",
            typeof(IEnumerable),
            typeof(BufferedItems),
            new PropertyMetadata(null, OnSourceChanged));

    private static readonly DependencyProperty ControllerProperty =
        DependencyProperty.RegisterAttached(
            "Controller",
            typeof(WpfBufferedItemsController),
            typeof(BufferedItems),
            new PropertyMetadata(null));

    public static IEnumerable? GetSource(DependencyObject d)
    {
        return (IEnumerable?)d.GetValue(SourceProperty);
    }

    public static void SetSource(DependencyObject d, IEnumerable? value)
    {
        d.SetValue(SourceProperty, value);
    }

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ItemsControl itemsControl)
            return;

        var controller = GetController(itemsControl);

        if (controller is null)
        {
            controller = new WpfBufferedItemsController(itemsControl);
            SetController(itemsControl, controller);
        }

        controller.SetSource((IEnumerable?)e.NewValue);
    }

    private static WpfBufferedItemsController? GetController(DependencyObject d)
    {
        return (WpfBufferedItemsController?)d.GetValue(ControllerProperty);
    }

    private static void SetController(DependencyObject d, WpfBufferedItemsController? value)
    {
        d.SetValue(ControllerProperty, value);
    }

    private sealed class WpfBufferedItemsController
    {
        private readonly ItemsControl itemsControl;
        private readonly BufferedItemsController controller;

        public WpfBufferedItemsController(ItemsControl itemsControl)
        {
            this.itemsControl = itemsControl;
            controller = new BufferedItemsController(
                new WpfItemsControlAdapter(itemsControl),
                new WpfDispatcherService(itemsControl.Dispatcher));

            itemsControl.Loaded += OnLoaded;
            itemsControl.Unloaded += OnUnloaded;
        }

        public void SetSource(IEnumerable? source)
        {
            controller.SetSource(source);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            controller.Detach();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetSource(GetSource(itemsControl));
        }
    }
}
