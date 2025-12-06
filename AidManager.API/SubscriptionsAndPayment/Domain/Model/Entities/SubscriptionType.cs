namespace AidManager.API.SubscriptionsAndPayment.Domain.Model.Entities;

public class SubscriptionType
{
    public int Id { get; set; }
    public string Type { get; set; }
    
    public SubscriptionType()
    {
        Type = string.Empty;
    }

    public SubscriptionType(string state)
    {
        Type = state;
    }
}