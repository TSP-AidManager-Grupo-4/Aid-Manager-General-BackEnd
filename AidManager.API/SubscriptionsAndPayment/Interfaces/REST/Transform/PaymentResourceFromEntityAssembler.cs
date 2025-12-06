using AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;

namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Transform;

public class PaymentResourceFromEntityAssembler
{
    public static PaymentResource ToResourceFromEntity(Domain.Model.Aggregates.Payment entity)
    {
        return new PaymentResource(
            entity.Id,
            entity.Amount,
            entity.Currency,
            entity.PaymentIntentId,
            entity.PaymentMethodId,
            entity.PaymentStatus,
            entity.UserId,
            entity.ReferenceId,
            entity.ReferenceType.ToString(),
            entity.CreatedAt,
            entity.UpdatedAt ?? DateTime.UtcNow
        );
    }
}