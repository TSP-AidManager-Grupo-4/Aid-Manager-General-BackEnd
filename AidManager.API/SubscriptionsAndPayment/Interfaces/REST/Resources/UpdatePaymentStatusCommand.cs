namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

public record UpdatePaymentStatusCommand(int id, string PaymentStatus);