using System.Diagnostics;

namespace AllForMessaging;

/// <summary>
/// A record struct implementation of <see cref="IMessage"/>.
/// </summary>
/// <param name="Content">The message content.</param>
/// <param name="Data">Optional data associated with the message.</param>
/// <param name="DataType">The type of the data.</param>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record struct Message(string? Content, object? Data, Type? DataType) : IMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> struct with null content and data.
    /// </summary>
    public Message() : this(null, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> struct with the specified content and optional data.
    /// </summary>
    /// <param name="content">The message content.</param>
    /// <param name="data">Optional data associated with the message.</param>
    public Message(string? content, object? data = null) : this(content, data, data?.GetType())
    {
    }

    private string GetDebuggerDisplay() => $"Content: {Content ?? "null"}, Data: {(Data is null ? "null" : Data.ToString() ?? "null")}, DataType: {DataType?.FullName ?? "null"}";
}