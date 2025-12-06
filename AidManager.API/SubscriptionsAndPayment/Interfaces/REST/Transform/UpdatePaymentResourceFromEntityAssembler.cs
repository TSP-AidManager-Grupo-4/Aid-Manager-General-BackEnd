using AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Transform;

public class UpdatePaymentResourceFromEntityAssembler
{
    public static UpdatePaymentResource ToResourceFromEntity(Domain.Model.Aggregates.Payment entity)
    {
        return new UpdatePaymentResource(
            entity.Amount,
            entity.Currency,
            entity.PaymentIntentId,
            entity.PaymentMethodId,
            entity.PaymentStatus,
            entity.UserId,
            entity.ReferenceId,
            entity.ReferenceType
        );
    }
}