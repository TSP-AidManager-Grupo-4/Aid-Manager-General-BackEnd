using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;
using AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Transform;

public class CreatePaymentCommandFromResourceAssembler
{
    public static CreatePaymentCommand ToCommandFromResource(CreatePaymentResource resource)
    {
        return new CreatePaymentCommand(
            resource.PaymentIntentId,
            resource.PaymentMethodId,
            resource.Amount,
            resource.Currency,
            resource.PaymentStatus,
            resource.UserId,
            resource.ReferenceId,
            resource.ReferenceType
            );
    }
}