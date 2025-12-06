using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Aggregates;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Repositories;

public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    Task<Subscription?> FindByUserIdAsync(int userId);
    Task<IEnumerable<Subscription>> ListAllAsync();
    Task<Subscription?> FindByIdAsync(int id);
}