namespace ASA_Server_Manager.Interfaces.ViewModels;

public interface IWindowViewModel : IViewModel
{
    /// <summary> Raised when the <see cref="IWindowViewModel" /> wishes to close its window. </summary>
    event EventHandler<bool?> CloseRequested;

    /// <summary> Called by the view when closing. </summary>
    /// <returns> <see langword="true" /> if the can close, otherwise <see langword="false" /> </returns>
    bool OnWindowClosing();
}