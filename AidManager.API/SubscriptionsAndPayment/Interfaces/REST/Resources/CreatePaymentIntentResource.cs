namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

public record CreatePaymentIntentResource(decimal Amount, string Currency, string PaymentMethodId);