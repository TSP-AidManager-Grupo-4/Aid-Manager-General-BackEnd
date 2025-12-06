using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;
using AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Transform;

public class CreatePaymentIntentCommandFromResourceAssembler
{
    public static CreatePaymentIntentCommand ToCommandFromResource(CreatePaymentIntentResource resource)
    {
        return new CreatePaymentIntentCommand(
            resource.Amount,
            resource.Currency,
            resource.PaymentMethodId
        );
    }
}