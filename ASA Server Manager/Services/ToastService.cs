using System.Windows;
using ASA_Server_Manager.Interfaces.Services;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace ASA_Server_Manager.Services;

public class ToastService : IToastService
{
    #region Private Fields

    private readonly Notifier _notifier;
    private bool _disposed;

    #endregion

    #region Public Constructors

    public ToastService()
        : this(null)
    {
    }

    public ToastService(Window window)
    {
        _notifier = CreateNotifier(window);
    }

    #endregion

    #region Private Properties

    private Corner ToastCorner => Corner.BottomRight;

    #endregion

    #region Public Methods

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _notifier?.Dispose();
    }

    public void ShowError(string message, Action onClickAction = null, Action onCloseClickedAction = null) => _notifier.ShowError(message, CreateMessageOptions(onClickAction, onCloseClickedAction));

    public void ShowInformation(string message, Action onClickAction = null, Action onCloseClickedAction = null) => _notifier.ShowInformation(message, CreateMessageOptions(onClickAction, onCloseClickedAction));

    public void ShowSuccess(string message, Action onClickAction = null, Action onCloseClickedAction = null) => _notifier.ShowSuccess(message, CreateMessageOptions(onClickAction, onCloseClickedAction));

    public void ShowWarning(string message, Action onClickAction = null, Action onCloseClickedAction = null) => _notifier.ShowWarning(message, CreateMessageOptions(onClickAction, onCloseClickedAction));

    #endregion

    #region Private Methods

    private MessageOptions CreateMessageOptions(Action onClickAction, Action onCloseClickedAction) =>
        new()
        {
            FreezeOnMouseEnter = true,
            UnfreezeOnMouseLeave = true,
            NotificationClickAction = _ => onClickAction?.Invoke(),
            CloseClickAction = _ => onCloseClickedAction?.Invoke()
        };

    private Notifier CreateNotifier(Window window)
    {
        IPositionProvider positionProvider = window is null
            ? new PrimaryScreenPositionProvider(ToastCorner, 10, 10)
            : new WindowPositionProvider(
                parentWindow: window,
                corner: ToastCorner,
                offsetX: 10,
                offsetY: 10
            );

        return new Notifier(cfg =>
        {
            cfg.PositionProvider = positionProvider;
            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(5), MaximumNotificationCount.FromCount(5));
            cfg.Dispatcher = Application.Current.Dispatcher;
        });
    }

    #endregion
}