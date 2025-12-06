using System.ComponentModel;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Model.ValueObjects;

public enum ESubscriptionStates
{
    [Description("Active")]
    Active = 1,
    [Description("Inactive")]
    Inactive = 2
}