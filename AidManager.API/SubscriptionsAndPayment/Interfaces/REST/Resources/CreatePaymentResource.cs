namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

public record CreatePaymentResource(
    string PaymentIntentId,
    string PaymentMethodId,
    decimal Amount,
    string Currency,
    string PaymentStatus,
    int UserId, 
    int ReferenceId,
    string ReferenceType
);