using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;
using AidManager.API.SubscriptionsAndPayment.Domain.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Services;
using UpdatePaymentStatusCommand = AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources.UpdatePaymentStatusCommand;
using Stripe;

namespace AidManager.API.SubscriptionsAndPayment.Application.Internal.CommandServices;

public class PaymentCommandService(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork) 
    : IPaymentCommandService
{
        
    public async Task<Domain.Model.Aggregates.Payment> Handle(CreatePaymentCommand command)
    {
        
        if (command.PaymentIntentId == null)
        {
            throw new ArgumentException("El PaymentIntentId no puede ser nulo.");
        }
        
        if (command.Amount <= 0)
        {
            throw new ArgumentException("El monto debe ser mayor que cero.");
        }

        var payment = new Domain.Model.Aggregates.Payment(command);

        // guardamos el pago en la base de datos
        await paymentRepository.AddAsync(payment);
        await unitOfWork.CompleteAsync();

        return payment;
    }

    public async Task<string> Handle(CreatePaymentIntentCommand command)
    {
        if (command.Amount <= 0)
        {
            throw new ArgumentException("El monto debe ser mayor que cero.");
        }

        long amountInCents = (long)(command.Amount * 100);

        // creamoss un PaymentIntent con Stripe
        var options = new PaymentIntentCreateOptions
        {
            Amount = amountInCents,
            Currency = command.Currency,
            ConfirmationMethod = "automatic",
        };

        // Configura la clave de API de Stripe
        var service = new PaymentIntentService();
        PaymentIntent paymentIntent = await service.CreateAsync(options);

        return paymentIntent.ClientSecret;
    }

    public async Task<string> Handle(ConfirmPaymentIntentCommand command)
    {
        var service = new PaymentIntentService();

        try
        {
            // obtenemos el PaymentIntent usando su ID
            var paymentIntent = await service.GetAsync(command.PaymentIntentId);

            var payment = await paymentRepository.FindByPaymentIntentIdAsync(paymentIntent.Id);

            if (payment != null)
            {
                payment.PaymentIntentId = paymentIntent.Id;
                payment.PaymentStatus = paymentIntent.Status;
                paymentRepository.Update(payment);
                await unitOfWork.CompleteAsync();
            }

            // devovlemos el estado del PaymentIntent (por eje: "succeeded", "failed", etc.)
            return paymentIntent.Status;
        }
        catch (StripeException ex)
        {
            throw new Exception($"Error al confirmar el PaymentIntent: {ex.Message}", ex);
        }
    }

    public Task Handle(UpdatePaymentStatusCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task Handle(UpdatePaymentCommand command)
    {
        var payment = await paymentRepository.GetPaymentByIdAsync(command.Id);
        if (payment == null)
        {
            throw new ArgumentException($"No se encontró un pago con el ID {command.Id}.");
        }
        payment.UpdatePayment(command);
        paymentRepository.Update(payment);
        await unitOfWork.CompleteAsync();
    }
}