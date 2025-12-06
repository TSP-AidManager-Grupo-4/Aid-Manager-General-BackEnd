using AidManager.API.SubscriptionsAndPayment.Domain.Model.Queries;
using AidManager.API.SubscriptionsAndPayment.Domain.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Services;

namespace AidManager.API.SubscriptionsAndPayment.Application.Internal.QueryServices;

public class PaymentQueryService(IPaymentRepository paymentRepository) : IPaymentQueryService
{
    public async Task<IEnumerable<Domain.Model.Aggregates.Payment>> Handle(GetAllPayments query)
    {
        return await paymentRepository.GetAllPaymentsAsync();
    }

    public async Task<IEnumerable<Domain.Model.Aggregates.Payment>> Handle(GetPaymentsByUserId query)
    {
        return await paymentRepository.GetPaymentsByUserIdAsync(query.UserId);
    }

    public async Task<IEnumerable<Domain.Model.Aggregates.Payment>> Handle(GetPaymentsBySubscriptionType query)
    {
        return await paymentRepository.GetPaymentsBySubscriptionType(query.SubscriptionType);
    }

    public Task<Domain.Model.Aggregates.Payment?> Handle(GetPaymentByPaymentIntentId query)
    {
        return paymentRepository.FindByPaymentIntentIdAsync(query.PaymentIntentId);
    }

    public Task<Domain.Model.Aggregates.Payment?> Handle(GetPaymentById query)
    {
        return paymentRepository.GetPaymentByIdAsync(query.PaymentId);
    }
}