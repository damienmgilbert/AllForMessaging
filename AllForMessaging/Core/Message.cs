namespace AllForMessaging.Core;

public record Message
{
    public Message(string? content, object? data, Type? dataType, Type? sender, Type? reciever)
    {
        Content = content;
        Data = data;
        DataType = dataType;
        Sender = sender;
        Reciever = reciever;
    }

    public string? Content { get; init; } = null;
    public object? Data { get; init; } = null;
    public Type? DataType { get; init; } = null;
    public Type? Sender { get; init; } = null;
    public Type? Reciever { get; init; } = null;
}
