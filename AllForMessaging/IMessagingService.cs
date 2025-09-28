namespace AllForMessaging;

/// <summary>
/// A messaging service that allows sending messages to registered listeners.
/// </summary>
public interface IMessagingService
{

    /// <summary>
    /// Sends a message to all registered listeners.
    /// </summary>
    /// <param name="message"></param>
    void SendMessage(IMessage message);

    /// <summary>
    /// Registers a listener to receive messages.
    /// </summary>
    /// <param name="listener"></param>
    void RegisterListener(IObserver<IMessage> listener);

    /// <summary>
    /// Unregisters a listener from receiving messages.
    /// </summary>
    /// <param name="listener"></param>
    void UnregisterListener(IObserver<IMessage> listener);
}
