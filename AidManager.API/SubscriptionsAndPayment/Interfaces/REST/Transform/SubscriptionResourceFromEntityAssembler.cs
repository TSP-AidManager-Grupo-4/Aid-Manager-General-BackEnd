using AidManager.API.SubscriptionsAndPayment.Domain.Model.Aggregates;
using AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Transform;

public class SubscriptionResourceFromEntityAssembler
{
    public static SubscriptionResource ToResourceFromEntity(   
        Subscription subscription)
    {
        return new SubscriptionResource(
            subscription.Id,
            subscription.UserId,
            subscription.SubscriptionTypeId,
            subscription.SubscriptionStateId,
            subscription.Currency,
            subscription.Amount,
            subscription.CreatedAt,
            subscription.UpdatedAt.GetValueOrDefault(),
            subscription.ExpirationDate);
    }
}