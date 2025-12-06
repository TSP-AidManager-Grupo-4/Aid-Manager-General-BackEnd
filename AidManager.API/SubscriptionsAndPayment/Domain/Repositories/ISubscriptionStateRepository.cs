using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Entities;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Repositories;

public interface ISubscriptionStateRepository : IBaseRepository<SubscriptionState>
{
    Task<bool> isSubcriptionStateExistsAsync(string? subscriptionState);
}