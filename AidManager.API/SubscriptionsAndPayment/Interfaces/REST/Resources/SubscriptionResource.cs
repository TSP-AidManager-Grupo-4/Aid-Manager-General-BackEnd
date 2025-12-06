namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

public record SubscriptionResource(
    int Id,
    int UserId,
    int SubscriptionTypeId,
    int SubscriptionStateId,
    string Currency,
    decimal Amount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime ExpirationDate
);