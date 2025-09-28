using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace AllForMessaging;

public partial class Observable
{
    /// <summary>
    /// Represents an unsubscriber for an observer in the <see cref="Observable"/> class.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    private class Unsubscriber : IDisposable
    {
        /// <summary>
        /// The logger instance for logging subscription and unsubscription events.
        /// </summary>
        private readonly ILogger<Observable>? _logger;

        /// <summary>
        /// The list of observers subscribed to the observable sequence.
        /// </summary>
        private readonly List<IObserver<IMessage>> _observers;

        /// <summary>
        /// The observer that is subscribed to the observable sequence.
        /// </summary>
        private readonly IObserver<IMessage> _observer;

        public Unsubscriber(List<IObserver<IMessage>> observers, IObserver<IMessage> observer, ILogger<Observable>? logger = null) : this()
        {
            _logger = logger;
            _observers = observers;
            _observer = observer;
        }

        private Unsubscriber()
        {
        }

        /// <summary>
        /// Unsubscribes the observer from the observable sequence.
        /// </summary>
        public void Dispose()
        {
            if (_observer is null)
            {
                _logger?.LogWarning("Observer is null, cannot unsubscribe");
                return;
            }

            bool observerIsSubscribed = _observers.Contains(_observer);
            if (observerIsSubscribed)
            {
                _logger?.LogDebug("Unsubscribing observer: {Observer}", _observer);
                _ = _observers.Remove(_observer);
            }
        }

        private string GetDebuggerDisplay() => $"Observer: {_observer}, Observers Count: {_observers?.Count ?? 0}";
    }
}
