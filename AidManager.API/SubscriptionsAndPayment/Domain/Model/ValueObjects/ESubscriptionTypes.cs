using System.ComponentModel;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Model.ValueObjects;

public enum ESubscriptionTypes
{
    [Description("Domestic")]
    Domestic = 1,
    [Description("Pro")]
    Pro = 2,
    [Description("Enterprise")]
    Enterprise = 3,
}