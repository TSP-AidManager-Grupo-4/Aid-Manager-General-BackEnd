using System.Net.Mime;
using AidManager.API.SubscriptionsAndPayment.Domain.Model.Queries;
using AidManager.API.SubscriptionsAndPayment.Domain.Services;
using AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Resources;
using AidManager.API.SubscriptionsAndPayment.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AidManager.API.SubscriptionsAndPayment.Interfaces.REST;

[Authorize]
[ApiController]
[Route("api/v1/payments")]
[Produces(MediaTypeNames.Application.Json)]
public class PaymentController(
    IPaymentCommandService paymentCommandService,
    IPaymentQueryService paymentQueryService)
    : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentResource resource)
    {
        var command = CreatePaymentCommandFromResourceAssembler.ToCommandFromResource(resource);
        var payment = await paymentCommandService.Handle(command);
        if (payment == null)
        {
            return BadRequest("Failed to create payment.");
        }
        var paymentResource = PaymentResourceFromEntityAssembler.ToResourceFromEntity(payment);
        return StatusCode(201, paymentResource);
    }
    
    [HttpPost("payment-intent")]
    [AllowAnonymous]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentResource resource)
    {
        var command = CreatePaymentIntentCommandFromResourceAssembler.ToCommandFromResource(resource);
        var clientSecret = await paymentCommandService.Handle(command);
        if (clientSecret == null)
        {
            return BadRequest("Failed to create payment intent.");
        }
        return StatusCode(201, new { ClientSecret = clientSecret });
    }
    
    [HttpPost("confirm-payment-intent")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmPaymentIntent([FromBody] ConfirmPaymentIntentResource resource)
    {
        var command = ConfirmPaymentIntentCommandFromResourceAssembler.ToCommandFromResource(resource);
        await paymentCommandService.Handle(command);
        return StatusCode(201, true);
    }
    
    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllPayments()
    {
        var query = new GetAllPayments();
        var payments = await paymentQueryService.Handle(query);
        var resources = payments.Select(PaymentResourceFromEntityAssembler.ToResourceFromEntity).ToList();
        return Ok(resources);
    }
    
    [HttpGet("{userId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPaymentsByUserId([FromRoute] int userId)
    {
        var query = new GetPaymentsByUserId(userId);
        var payments = await paymentQueryService.Handle(query);
        var resources = payments.Select(PaymentResourceFromEntityAssembler.ToResourceFromEntity).ToList();
        return Ok(resources);
    }
    
    [HttpGet("subscription-type")]
    public async Task<IActionResult> GetPaymentsBySubscriptionType([FromQuery] string subscriptionType)
    {
        var query = new GetPaymentsBySubscriptionType(subscriptionType);
        var payments = await paymentQueryService.Handle(query);
        var resources = payments.Select(PaymentResourceFromEntityAssembler.ToResourceFromEntity).ToList();
        return Ok(resources);
    }
    
    [HttpPut("{paymentId:int}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> UpdatePayment([FromBody] UpdatePaymentResource resource, [FromRoute] int paymentId)
    {
        var command = UpdatePaymentCommandFromResourceAssembler.ToCommandFromResource(paymentId, resource);
        await paymentCommandService.Handle(command);
        return Ok(true);
    }
}