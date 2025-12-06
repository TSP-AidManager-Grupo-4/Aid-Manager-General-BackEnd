using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.Shared.Infraestructure.Interfaces.ASP.Configuration.Extensions;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Entities;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.ValueObjects;
using AidManager.API.SubscriptionsAndPayment.Domain.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Services;

namespace AidManager.API.SubscriptionsAndPayment.Application.Internal.CommandServices;

public class SubscriptionStateCommandService(IUnitOfWork unitOfWork 
    , ISubscriptionStateRepository subscriptionStateRepository)
    : ISubscriptionStateCommandService
{
    public async Task Handle(SeedSubscriptionStatesCommand command)
    {
        var subscriptionStates = Enum.GetValues(typeof(ESubscriptionStates)).Cast<ESubscriptionStates>();
        foreach (var state in subscriptionStates)
        {
            var type = state.GetDescription();
            var exists = await subscriptionStateRepository.isSubcriptionStateExistsAsync(type);
            if (exists) continue;
            var subscriptionType = new SubscriptionState(type);
            await subscriptionStateRepository.AddAsync(subscriptionType);
            await unitOfWork.CompleteAsync();
        }
    }
}