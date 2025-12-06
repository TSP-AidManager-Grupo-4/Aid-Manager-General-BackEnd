using AidManager.API.SubscriptionsAndPayment.Domain.Model.Queries;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Services;

public interface IPaymentQueryService
{
    Task<IEnumerable<Model.Aggregates.Payment>> Handle(GetAllPayments query);
    Task<IEnumerable<Model.Aggregates.Payment>> Handle(GetPaymentsByUserId query);
    Task<IEnumerable<Model.Aggregates.Payment>> Handle(GetPaymentsBySubscriptionType query);
    
    Task<Model.Aggregates.Payment?> Handle(GetPaymentByPaymentIntentId query);
    Task<Model.Aggregates.Payment?> Handle(GetPaymentById query);
}