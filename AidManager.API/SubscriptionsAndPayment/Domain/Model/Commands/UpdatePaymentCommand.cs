namespace AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;

public record UpdatePaymentCommand(
    int Id,
    decimal Amount,
    string Currency,
    string PaymentIntentId,
    string PaymentMethodId,
    string PaymentStatus,
    int UserId,
    int ReferenceId,
    string ReferenceType);