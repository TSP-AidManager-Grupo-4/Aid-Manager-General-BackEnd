using UpdateSubscriptionTypeCommand = AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources.UpdateSubscriptionTypeCommand;

namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Transform;

public class UpdateSubscriptionTypeCommandFromResourceAssembler
{
    public static UpdateSubscriptionTypeCommand ToCommandFromResource(
        UpdateSubscriptionTypeCommand command)
    {
        return new UpdateSubscriptionTypeCommand(
            command.Id,
            command.UserId,
            command.SubscriptionTypeId
        );
    }
}