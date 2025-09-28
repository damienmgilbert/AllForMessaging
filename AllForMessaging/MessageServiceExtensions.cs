using Microsoft.Extensions.DependencyInjection;

namespace AllForMessaging;

/// <summary>
/// Extension methods for registering <see cref="IMessagingService"/> implementations with the dependency injection container.
/// </summary>
public static class MessageServiceExtensions
{
    /// <summary>
    /// Registers <see cref="IMessagingService"/> and <see cref="MessagingService"/> with scoped lifetime in the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMessageService(this IServiceCollection services)
    {
        _ = services.AddScoped<IMessagingService, MessagingService>();
        return services;
    }
}