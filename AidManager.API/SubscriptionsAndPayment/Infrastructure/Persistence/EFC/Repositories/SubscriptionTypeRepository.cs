using AidManager.API.Shared.Infraestructure.Persistence.EFC.Configuration;
using AidManager.API.Shared.Infraestructure.Persistence.EFC.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Entities;
using AidManager.API.SubscriptionsAndPayment.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AidManager.API.SubscriptionsAndPayment.Infrastructure.Persistence.EFC.Repositories;

public class SubscriptionTypeRepository(AppDBContext context) : BaseRepository<SubscriptionType>(context), ISubscriptionTypeRepository
{
    public async Task<bool> isSubcriptionTypeExistsAsync(string? subscriptionType)
    {
        return await context.Set<SubscriptionType>()
            .AnyAsync(ws => ws.Type == subscriptionType);
    }
}