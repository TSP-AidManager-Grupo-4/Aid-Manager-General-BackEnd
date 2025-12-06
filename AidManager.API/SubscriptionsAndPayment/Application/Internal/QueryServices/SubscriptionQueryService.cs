using AidManager.API.SubscriptionsAndPayment.Domain.Model.Aggregates;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Queries;
using AidManager.API.SubscriptionsAndPayment.Domain.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Services;

namespace AidManager.API.SubscriptionsAndPayment.Application.Internal.QueryServices;

public class SubscriptionQueryService(ISubscriptionRepository subscriptionRepository) : ISubscriptionQueryService
{
    public async Task<Subscription?> Handle(GetSubscriptionById query)
    {
        return await subscriptionRepository.FindByIdAsync(query.SubscriptionId);
    }

    public async Task<Subscription?> Handle(GetSubscriptionByUserIdQuery query)
    {
        return await subscriptionRepository.FindByUserIdAsync(query.UserId);
    }

    public async Task<IEnumerable<Subscription>> Handle(GetAllSubscriptions query)
    {
        return await subscriptionRepository.ListAllAsync();
    }
}