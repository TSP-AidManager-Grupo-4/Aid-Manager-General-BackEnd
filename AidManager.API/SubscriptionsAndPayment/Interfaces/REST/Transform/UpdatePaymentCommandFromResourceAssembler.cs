using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;
using AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Transform;

public class UpdatePaymentCommandFromResourceAssembler
{
    public static UpdatePaymentCommand ToCommandFromResource(int id, UpdatePaymentResource resource)
    {
        return new UpdatePaymentCommand(
            id,
            resource.Amount,
            resource.Currency,
            resource.PaymentIntentId,
            resource.PaymentMethodId,
            resource.PaymentStatus,
            resource.UserId,
            resource.ReferenceId,
            resource.ReferenceType
        );
    }
}