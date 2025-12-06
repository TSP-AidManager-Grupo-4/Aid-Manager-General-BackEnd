using AidManager.API.Shared.Interfaces.Helpers;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Model.Aggregates;

public class Subscription
{
    public int Id { get; }

    public int UserId { get; set; }

    public int SubscriptionTypeId { get; set; }

    public int SubscriptionStateId { get; set; }
    
    public decimal Amount { get; set; }
    
    public string Currency { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime ExpirationDate { get; set; }

    public Subscription()
    {
        UserId = 0;
        SubscriptionTypeId = 0;
        SubscriptionStateId = 0;
        Amount = 0;
        Currency = string.Empty;
        CreatedAt = DateTimeConverterHelper.ToNormalizeFormat(DateTime.UtcNow);
        UpdatedAt = null;
        ExpirationDate = DateTimeConverterHelper.ToNormalizeFormat(DateTime.UtcNow.AddMonths(1));
    }

    public Subscription(CreateSubscriptionCommand command)
    {
        UserId = command.UserId;
        SubscriptionTypeId = command.SubscriptionTypeId;
        SubscriptionStateId = 1; // default state
        Amount = command.Amount;
        Currency = command.Currency;
        CreatedAt = DateTimeConverterHelper.ToNormalizeFormat(DateTime.UtcNow);
        UpdatedAt = null;
        ExpirationDate = DateTimeConverterHelper.ToNormalizeFormat(DateTime.UtcNow.AddMonths(1)); // default to 1 month from now
    }

    public Subscription(UpdateSubscriptionTypeCommand command)
    {
        SubscriptionStateId = command.Id;
        UserId = command.UserId;
        SubscriptionTypeId = command.SubscriptionTypeId;
        UpdatedAt = DateTimeConverterHelper.ToNormalizeFormat(DateTime.UtcNow);
    }
}