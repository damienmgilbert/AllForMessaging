namespace AllForMessaging;

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
    /// 
    /// </summary>
    Type? DataType { get; set; }
}
