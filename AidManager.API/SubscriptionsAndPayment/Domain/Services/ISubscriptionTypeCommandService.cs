using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Services;

public interface ISubscriptionTypeCommandService
{
    Task Handle(SeedSubscriptionTypesCommand command);
}