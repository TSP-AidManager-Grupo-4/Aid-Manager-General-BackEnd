namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

public record UpdatePaymentResource(
    decimal Amount,
    string Currency,
    string PaymentIntentId,
    string PaymentMethodId,
    string PaymentStatus,
    int UserId,
    int ReferenceId,
    string ReferenceType
);