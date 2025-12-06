using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Services;

public interface IPaymentCommandService
{
    public Task<Model.Aggregates.Payment> Handle(CreatePaymentCommand command);
    public Task<string> Handle(CreatePaymentIntentCommand command);
    public Task<string> Handle(ConfirmPaymentIntentCommand command);
    
    public Task Handle(UpdatePaymentCommand command);
}