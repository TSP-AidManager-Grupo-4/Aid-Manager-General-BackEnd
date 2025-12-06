using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;
using AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Transform;

public class ConfirmPaymentIntentCommandFromResourceAssembler
{
    public static ConfirmPaymentIntentCommand ToCommandFromResource(
        ConfirmPaymentIntentResource resource)
    {
        return new ConfirmPaymentIntentCommand(resource.PaymentIntentId);
    }
}