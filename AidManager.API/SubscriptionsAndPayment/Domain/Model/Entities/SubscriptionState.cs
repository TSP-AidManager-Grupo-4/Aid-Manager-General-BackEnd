namespace AidManager.API.SubscriptionsAndPayment.Domain.Model.Entities;

public class SubscriptionState
{
    public int Id { get;  }
    public string State { get; private set; }
    
    public SubscriptionState()
    {
        State = string.Empty;
    }

    public SubscriptionState(string status)
    {
        State = status;
    }
}