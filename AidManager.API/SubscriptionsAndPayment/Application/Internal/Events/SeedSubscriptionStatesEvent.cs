using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;
using AidManager.API.SubscriptionsAndPayment.Domain.Services;

namespace AidManager.API.SubscriptionsAndPayment.Application.Internal.Events;

public static class SeedSubscriptionStatesEvent
{
    public static void On(this IServiceProvider provider, SeedSubscriptionStatesCommand command)
    {
        Console.WriteLine("Application Ready Event Triggered");
        using var scope = provider.CreateScope();
        var stateCommandService = scope.ServiceProvider.GetRequiredService<ISubscriptionStateCommandService>();
        stateCommandService.Handle(command).GetAwaiter().GetResult();
        Console.WriteLine("Subscription States Seeded Successfully");
    }
}