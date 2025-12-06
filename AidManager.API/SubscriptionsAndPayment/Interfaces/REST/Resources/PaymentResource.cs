namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

public record PaymentResource(
    int Id,
    decimal Amount,
    string Currency,
    string PaymentIntentId,
    string PaymentMethodId,
    string PaymentStatus,
    int UserId,
    int ReferenceId,
    string ReferenceType,
    DateTime CreatedAt,
    DateTime UpdatedAt
);