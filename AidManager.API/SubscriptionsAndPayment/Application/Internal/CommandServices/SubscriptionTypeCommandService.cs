using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.Shared.Infraestructure.Interfaces.ASP.Configuration.Extensions;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Entities;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.ValueObjects;
using AidManager.API.SubscriptionsAndPayment.Domain.Repositories;
using AidManager.API.SubscriptionsAndPayment.Domain.Services;

namespace AidManager.API.SubscriptionsAndPayment.Application.Internal.CommandServices;

public class SubscriptionTypeCommandService(
    IUnitOfWork unitOfWork,
    ISubscriptionTypeRepository subscriptionTypeRepository)
    : ISubscriptionTypeCommandService
{
    public async Task Handle(SeedSubscriptionTypesCommand command)
    {
        var subscriptionTypes = Enum.GetValues(typeof(ESubscriptionTypes)).Cast<ESubscriptionTypes>();
        foreach (var state in subscriptionTypes)
        {
            var type = state.GetDescription();
            var exists = await subscriptionTypeRepository.isSubcriptionTypeExistsAsync(type);
            if (exists) continue;
            var subscriptionType = new SubscriptionType(type);
            await subscriptionTypeRepository.AddAsync(subscriptionType);
            await unitOfWork.CompleteAsync();
        }
    }
}