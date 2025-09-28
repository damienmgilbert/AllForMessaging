using Microsoft.Extensions.Logging;

namespace AllForMessaging.Core;

public sealed class ParentView
{
    private readonly IMessenger<Message> _messenger;
    private readonly ILogger<ParentView>? _logger;

    public ParentView(IMessenger<Message> messenger, ILogger<ParentView>? logger = null)
    {
        _messenger = messenger;
        _logger = logger;
    }

    public async Task Start()
    {
        _ = await _messenger.ReadAsync();
        await _messenger.WriteAsync(new Message(
       content: "Hello from ParentView",
       data: new { Text = "This is a test message from ParentView.", Timestamp = DateTime.UtcNow },
       dataType: typeof(object),
       sender: typeof(ParentView),
       reciever: typeof(GrandChildView)));
    }
}
public sealed class ChildView
{
    private readonly IMessenger<Message> _messenger;
    private readonly ILogger<ChildView>? _logger;

    public ChildView(IMessenger<Message> messenger, ILogger<ChildView>? logger = null)
    {
        _messenger = messenger;
        _logger = logger;
    }

    public async Task Start()
    {
        _ = await _messenger.ReadAsync();
        await _messenger.WriteAsync(new Message(
        content: "Hello from ChildView",
        data: new { Text = "This is a test message from ChildView.", Timestamp = DateTime.UtcNow },
        dataType: typeof(object),
        sender: typeof(ChildView),
        reciever: typeof(ParentView)));
    }
}
public sealed class GrandChildView
{
    private readonly IMessenger<Message> _messenger;
    private readonly ILogger<GrandChildView>? _logger;
    public GrandChildView(IMessenger<Message> messenger, ILogger<GrandChildView>? logger = null)
    {
        _messenger = messenger;
        _logger = logger;
    }

    public async Task Start() => await _messenger.WriteAsync(new Message(
            content: "Hello from GrandChildView",
            data: new { Text = "This is a test message from GrandChildView.", Timestamp = DateTime.UtcNow },
            dataType: typeof(object),
            sender: typeof(GrandChildView),
            reciever: typeof(ChildView)));

    public async Task Stop() => _ = await _messenger.ReadAsync();
}
