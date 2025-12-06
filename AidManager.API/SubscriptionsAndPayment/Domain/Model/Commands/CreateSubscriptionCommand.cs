namespace AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;

public record CreateSubscriptionCommand(
    int UserId,
    int SubscriptionTypeId,
    int SubscriptionStateId,
    string Currency,
    decimal Amount
);