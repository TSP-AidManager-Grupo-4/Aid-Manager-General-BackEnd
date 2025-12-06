/*using EcoGuardian_Backend.SubscriptionsAndPayment.Domain.Model.Queries;
using EcoGuardian_Backend.SubscriptionsAndPayment.Domain.Services;

namespace EcoGuardian_Backend.SubscriptionsAndPayment.Interfaces.ACL.Service;

public class SubscriptionContextFacade(ISubscriptionQueryService subscriptionQueryService) : ISubscriptionContextFacade
{
    public async Task<bool> IsUserSubscribed(int userId)
    {
        var query = new GetSubscriptionByUserIdQuery(userId);
        var subscription = await subscriptionQueryService.Handle(query);
        return subscription != null;
    }
}*/