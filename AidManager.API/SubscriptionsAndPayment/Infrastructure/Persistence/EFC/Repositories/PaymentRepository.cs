using AidManager.API.Shared.Infraestructure.Persistence.EFC.Configuration;
using AidManager.API.Shared.Infraestructure.Persistence.EFC.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AidManager.API.SubscriptionsAndPayment.Infrastructure.Persistence.EFC.Repositories;

public class PaymentRepository(AppDBContext context) : BaseRepository<Domain.Model.Aggregates.Payment>(context), IPaymentRepository
{
    public async Task<IEnumerable<Domain.Model.Aggregates.Payment>> GetAllPaymentsAsync()
    {
        return await context.Set<Domain.Model.Aggregates.Payment>()
            .ToListAsync();
    }

    public async Task<IEnumerable<Domain.Model.Aggregates.Payment>> GetPaymentsByUserIdAsync(int userId)
    {
        return await context.Set<Domain.Model.Aggregates.Payment>()
            .Where(payment => payment.UserId == userId)
            .ToListAsync();
    }

    public Task<Domain.Model.Aggregates.Payment?> FindByPaymentIntentIdAsync(string paymentIntentId)
    {
        return context.Set<Domain.Model.Aggregates.Payment>()
            .FirstOrDefaultAsync(payment => payment.PaymentIntentId == paymentIntentId);
    }

    public async Task<IEnumerable<Domain.Model.Aggregates.Payment>> GetPaymentsBySubscriptionType(string subscriptionType)
    {
        return await context.Set<Domain.Model.Aggregates.Payment>()
            .Where(payment => payment.ReferenceType.ToString() == subscriptionType)
            .ToListAsync();
    }

    public Task<Domain.Model.Aggregates.Payment?> GetPaymentByIdAsync(int paymentId)
    {
        return context.Set<Domain.Model.Aggregates.Payment>()
            .FirstOrDefaultAsync(payment => payment.Id == paymentId);
    }
}