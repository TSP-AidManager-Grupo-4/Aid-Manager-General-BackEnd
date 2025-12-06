using AidManager.API.Shared.Interfaces.Helpers;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Commands;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.ValueObjects;

namespace AidManager.API.SubscriptionsAndPayment.Domain.Model.Aggregates;

public class Payment
{
    public int Id { get; private set; }
    
    public string PaymentIntentId { get; set; }
    
    public string PaymentMethodId { get; private set; }
    
    public decimal Amount { get; private set; }
    
    public string Currency { get; private set; }
    
    public string PaymentStatus { get; set; }
    
    public int UserId { get; private set; }
    
    public int ReferenceId { get; set; }
    
    public string ReferenceType { get; set; } = EPaymentReferenceType.None.ToString(); // Default value for ReferenceType
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime? UpdatedAt { get; private set; }

    public Payment()
    {
        CreatedAt = DateTime.UtcNow;
        PaymentStatus = "Pending";
        PaymentIntentId = string.Empty;
        PaymentMethodId = string.Empty;
        Currency = string.Empty;
        UpdatedAt = null;
        Amount = 0;
        UserId = 0;
        ReferenceId = 0;
        ReferenceType = EPaymentReferenceType.None.ToString(); // Default value for ReferenceType
        // we will use the enum and convert it to string when we need to return it as a resource or persist it in the database.
    }

    public Payment(CreatePaymentCommand command)
    {
        PaymentIntentId = command.PaymentIntentId;
        PaymentMethodId = command.PaymentMethodId;
        Amount = command.Amount;
        Currency = command.Currency;
        PaymentStatus = command.PaymentStatus;
        UserId = command.UserId;
        ReferenceId = command.ReferenceId;
        ReferenceType = command.ReferenceType;
        CreatedAt = DateTimeConverterHelper.ToNormalizeFormat(DateTime.UtcNow);
        UpdatedAt = null;
    }

    public Payment(CreatePaymentIntentCommand command)
    {
        Amount = command.Amount;
        Currency = command.Currency;
        PaymentMethodId = command.PaymentMethodId;
    }

    public Payment(ConfirmPaymentIntentCommand command)
    {
        PaymentIntentId = command.PaymentIntentId;
        PaymentStatus = "Confirmed";
    }
    
    public void UpdatePayment(UpdatePaymentCommand command)
    {
        Id = command.Id;
        Amount = command.Amount;
        Currency = command.Currency;
        PaymentIntentId = command.PaymentIntentId;
        PaymentMethodId = command.PaymentMethodId;
        PaymentStatus = command.PaymentStatus;
        UserId = command.UserId;
        ReferenceId = command.ReferenceId;
        ReferenceType = command.ReferenceType;
    }
}