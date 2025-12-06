namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

public record UpdateSubscriptionTypeCommand(
    int Id,
    int UserId,
    int SubscriptionTypeId
);