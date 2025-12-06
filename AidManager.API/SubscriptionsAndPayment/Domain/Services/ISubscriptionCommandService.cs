using AidManager.API.SubscriptionsAndPayment.Domain.Model.Aggregates;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Services;

public interface ISubscriptionCommandService
{
    public Task<Subscription?> Handle(CreateSubscriptionCommand command);
    public Task<Subscription?> Handle(UpdateSubscriptionTypeCommand command);
}