using System.Diagnostics;

namespace AllForMessaging.Old;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record struct Message<TData>(string? Content, TData? Data, Type Sender, Type Reciever) : IMessage<TData> where TData : class?
{
    public Message() : this(null, null, typeof(object), typeof(object))
    {
    }
    public Message(string? content, TData? data = null) : this(content, data, typeof(object), typeof(object))
    {
    }

    private string GetDebuggerDisplay()
    {
        return $"Content: {Content ?? "null"}, Data: {(Data is null ? "null" : Data.ToString() ?? "null")}, DataType: {typeof(TData).FullName}, Sender: {Sender.FullName}, Reciever: {Reciever.FullName}";
    }
}