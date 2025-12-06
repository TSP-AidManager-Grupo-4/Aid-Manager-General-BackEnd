using AidManager.API.Shared.Domain.Repositories;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Repositories;

public interface IPaymentRepository : IBaseRepository<Model.Aggregates.Payment>
{
    Task<IEnumerable<Model.Aggregates.Payment>> GetAllPaymentsAsync();
    Task<IEnumerable<Model.Aggregates.Payment>> GetPaymentsByUserIdAsync(int userId);
    Task<Model.Aggregates.Payment?> FindByPaymentIntentIdAsync(string paymentIntentId);
    Task<IEnumerable<Model.Aggregates.Payment>> GetPaymentsBySubscriptionType(string subscriptionType);
    Task<Model.Aggregates.Payment?> GetPaymentByIdAsync(int paymentId);
}