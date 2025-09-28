using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

namespace AllForMessaging;

/// <summary>
/// An observer that listens for messages and then can relay them to other systems.
/// This could be used for relaying messages between two different views, or forms in a user interfaces. It is capable of queuing messages for each listeners in case they are created/destroyed, such is the case with many user interface views. It is used in the <see cref="IMessagingService"/> which can is typically used through dependency injection.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Observer : IObserver<IMessage>, INotifyPropertyChanged, INotifyPropertyChanging, IEquatable<Observer>, IEqualityComparer<Observer>
{
    private readonly ILogger<Observer>? _logger;

    public Observer(ILogger<Observer>? logger = null)
        : this() => _logger = logger;
    private Observer()
    {
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Indicates the observable sequence has terminated (via completion or error).
    /// </summary>
    public bool IsCompleted
    {
        get;
        private set
        {
            if (value == field)
            {
                return;
            }

            OnPropertyChanging(nameof(IsCompleted));
            field = value;
            OnPropertyChanged(nameof(IsCompleted));
        }
    }

    /// <summary>
    /// The last error received from the observable sequence (if any).
    /// </summary>
    public Exception? Error
    {
        get;
        private set
        {
            if (ReferenceEquals(value, field))
            {
                return;
            }

            OnPropertyChanging(nameof(Error));
            field = value;
            OnPropertyChanged(nameof(Error));
        }
    }

    /// <summary>
    /// The most recent message received.
    /// </summary>
    public IMessage? LastMessage
    {
        get;
        private set
        {
            if (ReferenceEquals(value, field))
            {
                return;
            }

            OnPropertyChanging(nameof(LastMessage));
            field = value;
            OnPropertyChanged(nameof(LastMessage));
        }
    }

    public void OnCompleted()
    {
        _logger?.LogDebug("Observer sequence completed.");
        // Mark sequence as completed.
        IsCompleted = true;
    }

    public void OnError(Exception error)
    {
        ArgumentNullException.ThrowIfNull(error);
        _logger?.LogError(error, "Observer sequence encountered an error.");

        // Record the error and mark sequence as completed.
        Error = error;
        IsCompleted = true;
        _logger?.LogDebug("Observer sequence marked as completed due to error.");
    }

    public void OnNext(IMessage value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _logger?.LogDebug("Observer received message: {Message}", value);
        if (IsCompleted)
        {
            _logger?.LogDebug("Observer sequence is completed, ignoring message: {Message}", value);
            return; // Ignore further messages after termination.
        }

        // Update the last received message.
        LastMessage = value;
        _logger?.LogDebug("Observer updated LastMessage to: {Message}", value);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        _logger?.LogDebug("Property changed: {PropertyName}", propertyName);
    }

    protected virtual void OnPropertyChanging(string propertyName)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        _logger?.LogDebug("Property changing: {PropertyName}", propertyName);
    }

    private string GetDebuggerDisplay() =>
        // return a record version of the public properties for easy debugging
        $"{nameof(Observer)} {{ IsCompleted = {IsCompleted}, Error = {Error}, LastMessage = {LastMessage} }}";

    public bool Equals(Observer? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        bool isCompletedEqual = IsCompleted == other.IsCompleted;
        bool isErrorEqual = Equals(Error, other.Error);
        bool isLastMessageEqual = Equals(LastMessage, other.LastMessage);
        bool isGuidEqual = Guid == other.Guid;
        bool arePropertiesEqual = isCompletedEqual &&
               isErrorEqual &&
               isLastMessageEqual &&
               isGuidEqual;

        return arePropertiesEqual;
    }

    public Guid Guid { get; } = Guid.NewGuid();

    public override bool Equals(object? obj) => Equals(obj as Observer);

    public bool Equals(Observer? x, Observer? y) => (x is null && y is null) || (x is not null && y is not null && x.Equals(y));
    public int GetHashCode([DisallowNull] Observer obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        int hash = 17;
        hash = (hash * 23) + obj.IsCompleted.GetHashCode();
        hash = (hash * 23) + (obj.Error?.GetHashCode() ?? 0);
        hash = (hash * 23) + (obj.LastMessage?.GetHashCode() ?? 0);
        hash = (hash * 23) + obj.Guid.GetHashCode();
        return hash;
    }

    public override int GetHashCode() => GetHashCode(this);
}

