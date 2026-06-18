using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Olivercode.WPFastr.Collections;

namespace Olivercode.WPFastr.Behaviors;

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
            typeof(BufferedItemsController),
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
            controller = new BufferedItemsController(itemsControl);
            SetController(itemsControl, controller);
        }

        controller.SetSource((IEnumerable?)e.NewValue);
    }

    private static BufferedItemsController? GetController(DependencyObject d)
    {
        return (BufferedItemsController?)d.GetValue(ControllerProperty);
    }

    private static void SetController(DependencyObject d, BufferedItemsController? value)
    {
        d.SetValue(ControllerProperty, value);
    }

    private sealed class BufferedItemsController
    {
        private readonly ItemsControl itemsControl;
        private ObservableCollection<object?> visibleItems = new();
        private INotifyCollectionChanged? currentObservableSource;
        private INotifyCollectionReplacement? currentReplacementSource;
        private bool replacementInProgress;
        private long detachedClearVersion;

        public BufferedItemsController(ItemsControl itemsControl)
        {
            this.itemsControl = itemsControl;
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, visibleItems);
            itemsControl.Loaded += OnLoaded;
            itemsControl.Unloaded += OnUnloaded;
        }

        public void SetSource(IEnumerable? source)
        {
            DetachSourceEvents();

            replacementInProgress = false;
            visibleItems = CopyItems(source);
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, visibleItems);

            currentObservableSource = source as INotifyCollectionChanged;

            if (currentObservableSource is not null)
                currentObservableSource.CollectionChanged += Source_OnCollectionChanged;

            currentReplacementSource = source as INotifyCollectionReplacement;

            if (currentReplacementSource is not null)
                currentReplacementSource.ReplacementStarting += Source_OnReplacementStarting;
        }

        private void Source_OnReplacementStarting(object? sender, EventArgs e)
        {
            BeginReplacement();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DetachSourceEvents();
            detachedClearVersion++;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetSource(GetSource(itemsControl));
        }

        private void DetachSourceEvents()
        {
            if (currentObservableSource is not null)
                currentObservableSource.CollectionChanged -= Source_OnCollectionChanged;

            if (currentReplacementSource is not null)
                currentReplacementSource.ReplacementStarting -= Source_OnReplacementStarting;

            currentObservableSource = null;
            currentReplacementSource = null;
        }

        private void Source_OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (currentReplacementSource is null)
            {
                ApplyChange(e);
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItems(e.NewItems, e.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    if (!replacementInProgress || ContainsVisibleItem(e.OldItems))
                        BeginReplacement();
                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    ApplyChange(e);
                    break;
            }
        }

        private void BeginReplacement()
        {
            replacementInProgress = true;
            var oldItems = visibleItems;
            visibleItems = new ObservableCollection<object?>();
            itemsControl.SetValue(ItemsControl.ItemsSourceProperty, visibleItems);
            ClearDetachedItemsAsync(oldItems, ++detachedClearVersion);
        }

        private void ApplyChange(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItems(e.NewItems, e.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(e.OldItems, e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    RemoveItems(e.OldItems, e.OldStartingIndex);
                    AddItems(e.NewItems, e.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Move:
                    MoveItems(e.OldItems, e.OldStartingIndex, e.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    visibleItems.Clear();
                    break;
            }
        }

        private void AddItems(IList? items, int startingIndex)
        {
            if (items is null)
                return;

            var insertIndex = startingIndex >= 0
                ? Math.Min(startingIndex, visibleItems.Count)
                : visibleItems.Count;

            foreach (var item in items)
                visibleItems.Insert(insertIndex++, item);
        }

        private void RemoveItems(IList? items, int startingIndex)
        {
            if (items is null)
                return;

            if (startingIndex >= 0)
            {
                for (var i = 0; i < items.Count && startingIndex < visibleItems.Count; i++)
                    visibleItems.RemoveAt(startingIndex);

                return;
            }

            foreach (var item in items)
                visibleItems.Remove(item);
        }

        private void MoveItems(IList? items, int oldStartingIndex, int newStartingIndex)
        {
            if (items is null || oldStartingIndex < 0 || newStartingIndex < 0)
                return;

            for (var i = 0; i < items.Count; i++)
            {
                var item = visibleItems[oldStartingIndex];
                visibleItems.RemoveAt(oldStartingIndex);
                visibleItems.Insert(Math.Min(newStartingIndex + i, visibleItems.Count), item);
            }
        }

        private void ClearDetachedItemsAsync(ObservableCollection<object?> items, long clearVersion)
        {
            itemsControl.Dispatcher.BeginInvoke(() =>
            {
                if (clearVersion == detachedClearVersion && items.Count > 0)
                {
                    items.RemoveAt(0);
                    ClearDetachedItemsAsync(items, clearVersion);
                }
            }, DispatcherPriority.ApplicationIdle);
        }

        private bool ContainsVisibleItem(IList? items)
        {
            if (items is null)
                return true;

            foreach (var item in items)
            {
                if (visibleItems.Contains(item))
                    return true;
            }

            return false;
        }

        private static ObservableCollection<object?> CopyItems(IEnumerable? source)
        {
            var items = new ObservableCollection<object?>();

            if (source is null)
                return items;

            foreach (var item in source)
                items.Add(item);

            return items;
        }
    }
}
