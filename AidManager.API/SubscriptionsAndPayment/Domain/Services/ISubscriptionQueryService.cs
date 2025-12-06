using AidManager.API.SubscriptionsAndPayment.Domain.Model.Aggregates;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Queries;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Services;

public interface ISubscriptionQueryService
{
    Task<Subscription?> Handle(GetSubscriptionById query);
    Task<Subscription?> Handle(GetSubscriptionByUserIdQuery query);
    Task<IEnumerable<Subscription>> Handle(GetAllSubscriptions query);
}