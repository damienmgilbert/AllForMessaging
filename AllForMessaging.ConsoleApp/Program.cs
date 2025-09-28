using AllForMessaging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

ServiceCollection services = new();
services.AddMessageService();
services.AddLogging(services =>
{
    _ = services.AddConsole();
    _ = services.AddDebug();
    _ = services.SetMinimumLevel(LogLevel.Debug);
});
using ServiceProvider provider = services.BuildServiceProvider();

ILogger<Program>? programLogger = provider.GetService<ILogger<Program>>();
IMessagingService messagingService = provider.GetRequiredService<IMessagingService>();
ILogger<Observer>? observerLogger = provider.GetService<ILogger<Observer>>();
IObserver<IMessage> listener = new Observer(observerLogger);
messagingService.RegisterListener(listener);
messagingService.SendMessage(new Message("Hello, World!"));
programLogger?.LogDebug("Unregistering listener: {Listener}", listener);
Console.ReadLine();
