namespace AllForMessaging;

public interface IMessage<T>
    where T : class?, new()
{
    string? Content { get; set; }
    T? Data { get; set; }
}
