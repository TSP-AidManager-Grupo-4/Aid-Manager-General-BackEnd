using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Aggregates;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.ValueObjects;
using AidManager.API.SubscriptionsAndPayment.Domain.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Services;

namespace AidManager.API.SubscriptionsAndPayment.Application.Internal.CommandServices;

public class SubscriptionCommandService(
    ISubscriptionRepository subscriptionRepository,
    ISubscriptionStateRepository subscriptionStateRepository,
    ISubscriptionTypeRepository subscriptionTypeRepository,
    IUnitOfWork unitOfWork
) : ISubscriptionCommandService
{
    public async Task<Subscription?> Handle(CreateSubscriptionCommand command)
    {
        
        // TODO: Validar si el usuario ya tiene una suscripción activa

        try
        {
            var subscription = new Subscription(command);
            await subscriptionRepository.AddAsync(subscription);
            await unitOfWork.CompleteAsync();

            return subscription;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error al crear la suscripción.", ex);
        }
    }


    public async Task<Subscription?> Handle(UpdateSubscriptionTypeCommand command)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.Id);
        if (subscription == null)
        {
            throw new BadHttpRequestException("No se encontró la suscripción con el ID proporcionado.");
        }
        
        var subscriptionType = await subscriptionTypeRepository.FindByIdAsync(command.SubscriptionTypeId);
        if (subscriptionType == null)
        {
            throw new BadHttpRequestException("No se encontró el tipo de suscripción con el ID proporcionado.");
        }

        // actualizamos los campos
        subscription.SubscriptionTypeId = command.SubscriptionTypeId;
        subscription.Amount = GetAmountForSubscriptionType(command.SubscriptionTypeId);
        subscription.SubscriptionStateId = (int)ESubscriptionStates.Inactive;
        subscription.UpdatedAt = DateTime.UtcNow;

        try 
        {
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error al actualizar la suscripción.", ex);
        }

        return subscription;
    }


    // metodo privado para obtener el monto de la suscripción según el tipo (esto para la actualizacion de suscripción)
    private decimal GetAmountForSubscriptionType(int subscriptionTypeId)
    {
        switch (subscriptionTypeId)
        {
            case 1: return 50;
            case 2: return 100;
            case 3: return 500;
            default:
                throw new InvalidOperationException("01101100 01101100 01101111 01110010 01100001 00100000 01101101 01100001 01110100 01101001");
        }
    }
}