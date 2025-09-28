using AllForMessaging.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Message = AllForMessaging.Core.Message;
ServiceCollection services = new();
services.AddSingleton<IMessenger<Message>, Messenger<Message>>();
services.AddTransient<ParentView>();
services.AddTransient<ChildView>();
services.AddTransient<GrandChildView>();
services.AddLogging(services =>
{
    _ = services.AddConsole();
    _ = services.AddDebug();
    _ = services.SetMinimumLevel(LogLevel.Debug);
});
using ServiceProvider provider = services.BuildServiceProvider();

ILogger<Program> programLogger = provider.GetRequiredService<ILogger<Program>>();
IMessenger<Message> messenger = provider.GetRequiredService<IMessenger<Message>>();
ParentView parentView = provider.GetRequiredService<ParentView>();
ChildView childView = provider.GetRequiredService<ChildView>();
GrandChildView grandChildView = provider.GetRequiredService<GrandChildView>();
programLogger.LogInformation("Starting messaging demo...");

//await messenger.WriteAsync(new Message(
//    content: "Hello, World!",
//    data: new { Text = "This is a test message.", Timestamp = DateTime.UtcNow },
//    dataType: typeof(object),
//    sender: typeof(Program),
//    reciever: typeof(Program)));

//Message? message = await messenger.ReadAsync();

//if (message is null)
//{
//    programLogger.LogWarning("No message received.");
//}
//else
//{
//    programLogger.LogInformation("Received message: {Message}", message);
//}

await grandChildView.Start();
await childView.Start();
await parentView.Start();
await grandChildView.Stop();

Console.ReadLine();
