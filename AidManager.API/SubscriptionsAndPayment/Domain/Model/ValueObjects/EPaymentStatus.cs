namespace AidManager.API.SubscriptionsAndPayment.Domain.Model.ValueObjects;

public enum EPaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Cancelled = 3
}