using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Olicode.WPFastr.Behaviors;

public static class ItemsControlScroll
{
    public static readonly DependencyProperty ResetOnItemsResetProperty =
        DependencyProperty.RegisterAttached(
            "ResetOnItemsReset",
            typeof(bool),
            typeof(ItemsControlScroll),
            new PropertyMetadata(false, OnResetOnItemsResetChanged));

    private static readonly DependencyProperty ControllerProperty =
        DependencyProperty.RegisterAttached(
            "Controller",
            typeof(ScrollResetController),
            typeof(ItemsControlScroll),
            new PropertyMetadata(null));

    public static bool GetResetOnItemsReset(DependencyObject d)
    {
        return (bool)d.GetValue(ResetOnItemsResetProperty);
    }

    public static void SetResetOnItemsReset(DependencyObject d, bool value)
    {
        d.SetValue(ResetOnItemsResetProperty, value);
    }

    private static void OnResetOnItemsResetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ItemsControl itemsControl)
            return;

        var controller = (ScrollResetController?)itemsControl.GetValue(ControllerProperty);

        if ((bool)e.NewValue)
        {
            if (controller is null)
            {
                controller = new ScrollResetController(itemsControl);
                itemsControl.SetValue(ControllerProperty, controller);
            }

            controller.Enable();
            return;
        }

        controller?.Disable();
        itemsControl.ClearValue(ControllerProperty);
    }

    private sealed class ScrollResetController
    {
        private static readonly DependencyPropertyDescriptor ItemsSourceDescriptor =
            DependencyPropertyDescriptor.FromProperty(
                ItemsControl.ItemsSourceProperty,
                typeof(ItemsControl));

        private readonly ItemsControl itemsControl;
        private INotifyCollectionChanged? observableSource;
        private bool enabled;
        private bool attached;
        private int resetVersion;

        public ScrollResetController(ItemsControl itemsControl)
        {
            this.itemsControl = itemsControl;
        }

        public void Enable()
        {
            if (enabled)
                return;

            enabled = true;
            itemsControl.Loaded += OnLoaded;
            itemsControl.Unloaded += OnUnloaded;

            if (itemsControl.IsLoaded)
                Attach();
        }

        public void Disable()
        {
            if (!enabled)
                return;

            enabled = false;
            itemsControl.Loaded -= OnLoaded;
            itemsControl.Unloaded -= OnUnloaded;
            Detach();
            resetVersion++;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Attach();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Detach();
        }

        private void Attach()
        {
            if (attached)
                return;

            attached = true;
            ItemsSourceDescriptor.AddValueChanged(itemsControl, OnItemsSourceChanged);
            SubscribeToItemsSource();
        }

        private void Detach()
        {
            if (!attached)
                return;

            attached = false;
            ItemsSourceDescriptor.RemoveValueChanged(itemsControl, OnItemsSourceChanged);
            UnsubscribeFromItemsSource();
        }

        private void OnItemsSourceChanged(object? sender, EventArgs e)
        {
            UnsubscribeFromItemsSource();
            SubscribeToItemsSource();
            QueueScrollToTop();
        }

        private void SubscribeToItemsSource()
        {
            observableSource = itemsControl.ItemsSource as INotifyCollectionChanged;

            if (observableSource is not null)
                observableSource.CollectionChanged += OnCollectionChanged;
        }

        private void UnsubscribeFromItemsSource()
        {
            if (observableSource is not null)
                observableSource.CollectionChanged -= OnCollectionChanged;

            observableSource = null;
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
                QueueScrollToTop();
        }

        private void QueueScrollToTop()
        {
            var version = ++resetVersion;

            itemsControl.Dispatcher.BeginInvoke(() =>
            {
                if (!enabled || version != resetVersion)
                    return;

                FindVisualChild<ScrollViewer>(itemsControl)?.ScrollToTop();
            }, DispatcherPriority.ContextIdle);
        }

        private static T? FindVisualChild<T>(DependencyObject parent)
            where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T match)
                    return match;

                var descendant = FindVisualChild<T>(child);

                if (descendant is not null)
                    return descendant;
            }

            return null;
        }
    }
}
