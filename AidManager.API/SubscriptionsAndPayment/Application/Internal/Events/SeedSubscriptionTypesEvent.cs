using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;
using AidManager.API.SubscriptionsAndPayment.Domain.Services;

namespace AidManager.API.SubscriptionsAndPayment.Application.Internal.Events;

public static class SeedSubscriptionTypesEvent
{
    public static void On(this IServiceProvider provider, SeedSubscriptionTypesCommand command)
    {
        // Sembrar tipos de suscripción
        using var scope = provider.CreateScope();
        var typeCommandService = scope.ServiceProvider.GetRequiredService<ISubscriptionTypeCommandService>();
        typeCommandService.Handle(command).GetAwaiter().GetResult();
        Console.WriteLine("Subscription Types Seeded Successfully");
    }
}