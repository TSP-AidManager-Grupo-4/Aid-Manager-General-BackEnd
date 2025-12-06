using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Entities;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Repositories;

public interface ISubscriptionTypeRepository : IBaseRepository<SubscriptionType>
{
    Task<bool> isSubcriptionTypeExistsAsync(string? subscriptionType);
}