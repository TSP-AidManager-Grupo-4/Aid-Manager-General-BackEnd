using AidManager.API.Shared.Infraestructure.Persistence.EFC.Configuration;
using AidManager.API.Shared.Infraestructure.Persistence.EFC.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Entities;
using AidManager.API.SubscriptionsAndPayment.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AidManager.API.SubscriptionsAndPayment.Infrastructure.Persistence.EFC.Repositories;

public class SubscriptionStateRepository(AppDBContext context) : BaseRepository<SubscriptionState>(context), ISubscriptionStateRepository
{
    public async Task<bool> isSubcriptionStateExistsAsync(string? subscriptionState)
    {
        return await context.Set<SubscriptionState>()
            .AnyAsync(ws => ws.State == subscriptionState);
    }
}