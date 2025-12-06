using AidManager.API.Shared.Infraestructure.Persistence.EFC.Configuration;
using AidManager.API.Shared.Infraestructure.Persistence.EFC.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Aggregates;
using AidManager.API.SubscriptionsAndPayment.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AidManager.API.SubscriptionsAndPayment.Infrastructure.Persistence.EFC.Repositories;

public class SubscriptionRepository(AppDBContext context) : BaseRepository<Subscription>(context), ISubscriptionRepository
{
    public async Task<Subscription?> FindByUserIdAsync(int userId)
    {
        return await context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<IEnumerable<Subscription>> ListAllAsync()
    {
        return await context.Set<Subscription>()
            .ToListAsync();
    }

    public Task<Subscription?> FindByIdAsync(int id)
    {
        return context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}