using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace ASA_Server_Manager.Behaviors;

public class PassMouseWheelEventToParentBehavior : Behavior<UIElement>
{
    #region Private Fields

    private IDisposable _subscription;

    #endregion

    #region Protected Methods

    protected override void OnAttached()
    {
        base.OnAttached();

        // We cast AssociatedObject to a local variable to prevent null reference exceptions when
        // unsubscribing. When the subscription is being disposed, it's not guaranteed to run
        // immediately, so AssociatedObject might be null. By using the local variable instead of
        // the field, we're forcing it to capture the actual object instead of just using the Property!
        var associatedObject = AssociatedObject;

        _subscription = Observable
            .FromEventPattern<MouseWheelEventHandler, MouseWheelEventArgs>(
                h => associatedObject.PreviewMouseWheel += h,
                h => associatedObject.PreviewMouseWheel -= h
            )
            .Subscribe(e => OnPreviewMouseWheel(e.Sender, e.EventArgs));
    }

    protected override void OnDetaching()
    {
        _subscription.Dispose();
        base.OnDetaching();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Finds the first visual child of a specific type in the visual tree of a DependencyObject.
    /// </summary>
    /// <typeparam name="T"> The type of the visual child to find. </typeparam>
    /// <param name="parent"> The parent DependencyObject. </param>
    /// <returns>
    /// The first visual child of the specified type, or null if no such child is found.
    /// </returns>
    private static T GetVisualChild<T>(DependencyObject parent)
        where T : Visual
    {
        var child = default(T);

        var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < numVisuals; i++)
        {
            var v = (Visual) VisualTreeHelper.GetChild(parent, i);

            child = v as T ?? GetVisualChild<T>(v);

            if (child != null)
            {
                break;
            }
        }

        return child;
    }

    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Handled || sender is null)
        {
            return;
        }

        // Check if the ScrollViewer was found

        if (GetVisualChild<ScrollViewer>(AssociatedObject) is not { } scrollViewer)
            return;

        var scrollPos = scrollViewer.ContentVerticalOffset;
        if ((scrollPos == scrollViewer.ScrollableHeight && e.Delta < 0)
            || (scrollPos == 0 && e.Delta > 0))
        {
            // Set the event as handled to prevent the AssociatedObject from intercepting it.
            e.Handled = true;

            // We need to create a new event as we want our control as the source, as the current
            // source is not guaranteed to be our control (it could be from a ListViewItem, etc...).
            var eventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender,
            };

            (sender as UIElement)?.RaiseEvent(eventArgs);
        }
    }

    #endregion
}