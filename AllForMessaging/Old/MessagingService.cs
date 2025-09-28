using System.Collections.Concurrent;
using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace AllForMessaging.Old;

/// <summary>
/// A messaging service that allows sending messages to registered listeners.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class MessagingService : IMessagingService
{
    /// <summary>
    /// The observable instance to manage subscriptions and notifications.
    /// </summary>
    private readonly Observable _observable = new();

    /// <summary>
    /// A thread-safe dictionary to manage subscriptions and ensure no duplicate registrations.
    /// </summary>
    private readonly ConcurrentDictionary<IObserver<IMessage>, IDisposable> _subscriptions = new();
    private readonly ILogger<MessagingService>? _logger;

    private MessagingService()
    {
    }

    public MessagingService(ILogger<MessagingService>? logger = null)
        : this() => _logger = logger;

    /// <inheritdoc/>
    public void SendMessage(IMessage message)
    {
        // Implementation to send message to all registered listeners
        ArgumentNullException.ThrowIfNull(message);
        _logger?.LogDebug("Sending message: {Message}", message);
        _observable.Notify(message);
    }

    /// <inheritdoc/>
    public void RegisterListener(IObserver<IMessage> listener)
    {
        ArgumentNullException.ThrowIfNull(listener);
        _logger?.LogDebug("Registering listener: {Listener}", listener);
        IDisposable subscription = _observable.Subscribe(listener);

        // Avoid duplicate registrations; dispose new subscription if already registered
        bool isAlreadyRegistered = !_subscriptions.TryAdd(listener, subscription);
        if (isAlreadyRegistered)
        {
            _logger?.LogWarning("Listener {Listener} is already registered. Ignoring duplicate registration.", listener);
            subscription.Dispose();
        }
    }

    /// <inheritdoc/>
    public void UnregisterListener(IObserver<IMessage> listener)
    {
        // Implementation to unregister listener
        ArgumentNullException.ThrowIfNull(listener);

        _logger?.LogDebug("Unregistering listener: {Listener}", listener);

        bool isUnregistered = _subscriptions.TryRemove(listener, out IDisposable? subscription);

        if (isUnregistered)
        {
            _logger?.LogDebug("Listener {Listener} unregistered successfully.", listener);
            subscription?.Dispose();
        }
    }

    private string GetDebuggerDisplay() => $"Subscriptions: {_subscriptions.Count}, Observable: {_observable}";
}
