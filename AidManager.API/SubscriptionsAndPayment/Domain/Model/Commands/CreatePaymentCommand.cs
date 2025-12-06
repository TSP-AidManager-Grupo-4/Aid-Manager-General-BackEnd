namespace AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;

public record CreatePaymentCommand(
    string PaymentIntentId,
    string PaymentMethodId,
    decimal Amount,
    string Currency,
    string PaymentStatus,
    int UserId,
    int ReferenceId,
    string ReferenceType
);