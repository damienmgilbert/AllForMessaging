namespace AllForMessaging.Old;

/// <summary>
/// A interface for passing generic messages.
/// </summary>
public interface IMessage
{
    /// <summary>
    /// The message content.
    /// </summary>
    string? Content { get; set; }

    /// <summary>
    /// Optional data associated with the message.
    /// </summary>
    object? Data { get; set; }

    /// <summary>
    /// The type of the data associated with the message.
    /// </summary>
    Type? DataType { get; set; }

    /// <summary>
    /// The sender of the message.
    /// </summary>
    Type Sender { get; set; }

    /// <summary>
    /// The receiver of the message.
    /// </summary>
    Type Reciever { get; set; }
}

public interface IMessage<TData> where TData : class?
{
    string? Content { get; set; }
    TData? Data { get; set; }
    Type Sender { get; set; }
    Type Reciever { get; set; }
}
