using System.Diagnostics;

using AllForMessaging.Old;

using Microsoft.Extensions.Logging;

namespace AllForMessaging;

/// <summary>
/// Represents an observable sequence of <see cref="IMessage"/> objects.
/// Allows observers to subscribe and receive notifications when messages are sent, completed, or an error occurs.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial class Observable : IObservable<IMessage>
{
    private readonly List<IObserver<IMessage>> _observers = [];
    private readonly ILogger<Observable>? _logger;

    public Observable(ILogger<Observable>? logger = null)
        : this() => _logger = logger;

    private Observable()
    {
    }

    /// <summary>
    /// Subscribes the specified observer to receive notifications from this observable sequence.
    /// </summary>
    /// <param name="observer">The observer to subscribe.</param>
    /// <returns>An <see cref="IDisposable"/> that can be used to unsubscribe the observer.</returns>
    public IDisposable Subscribe(IObserver<IMessage> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        _logger?.LogDebug("Subscribing observer: {Observer}", observer);
        bool shouldAddObserver = !_observers.Contains(observer);
        if (shouldAddObserver)
        {
            _logger?.LogDebug("Observer added: {Observer}", observer);
            _observers.Add(observer);
        }
        var subscription = new Unsubscriber(_observers, observer, _logger);
        return subscription;
    }

    /// <summary>
    /// Notifies all subscribed observers with the specified message.
    /// </summary>
    /// <param name="message">The message to send to observers.</param>
    public void Notify(IMessage message)
    {
        _logger?.LogDebug("Notifying observers with message: {Message}", message);
        IObserver<IMessage>[] observers = [.. _observers];
        foreach (IObserver<IMessage> observer in observers)
        {
            _logger?.LogDebug("Notifying observer: {Observer} with message: {Message}", observer, message);
            observer.OnNext(message);
        }
    }

    /// <summary>
    /// Notifies all subscribed observers that the sequence has completed and clears the observer list.
    /// </summary>
    public void Complete()
    {
        _logger?.LogDebug("Completing observable");
        IObserver<IMessage>[] observers = [.. _observers];
        foreach (IObserver<IMessage> observer in observers)
        {
            _logger?.LogDebug("Completing observer: {Observer}", observer);
            observer.OnCompleted();
        }
        _logger?.LogDebug("Clearing observers");
        _observers.Clear();
    }

    /// <summary>
    /// Notifies all subscribed observers that an error has occurred and clears the observer list.
    /// </summary>
    /// <param name="error">The exception to send to observers.</param>
    public void Error(Exception error)
    {
        _logger?.LogError(error, "Error occurred in observable");
        IObserver<IMessage>[] observers = [.. _observers];
        foreach (IObserver<IMessage> observer in observers)
        {
            _logger?.LogDebug("Notifying observer: {Observer} of error: {Error}", observer, error);
            observer.OnError(error);
        }
        _logger?.LogDebug("Clearing observers");
        _observers.Clear();
    }

    private string GetDebuggerDisplay() => $"Observers: {_observers.Count}";
}
