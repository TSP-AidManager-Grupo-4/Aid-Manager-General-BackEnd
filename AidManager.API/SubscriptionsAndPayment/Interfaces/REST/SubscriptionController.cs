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
[Route("api/v1/subscriptions")]
[Produces(MediaTypeNames.Application.Json)]
public class SubscriptionController(
    ISubscriptionCommandService subscriptionCommandService,
    ISubscriptionQueryService subscriptionQueryService)
    : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreatePayment([FromBody] CreateSubscriptionResource resource)
    {
        var command = CreateSubscriptionCommandFromResourceAssembler.ToCommandFromResource(resource);
        var subscription = await subscriptionCommandService.Handle(command);
        if (subscription == null)
        {
            return BadRequest("Failed to create subscription.");
        }

        var subscriptionResource = SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(subscription);
        return Ok(subscriptionResource);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllSubscriptions()
    {
        var query = new GetAllSubscriptions();
        var payments = await subscriptionQueryService.Handle(query);
        var resources = payments.Select(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity).ToList();
        return Ok(resources);
    }

    [HttpGet("user/{userId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSubscriptionByUserId([FromRoute] int userId)
    {
        var query = new GetSubscriptionByUserIdQuery(userId);
        var subscription = await subscriptionQueryService.Handle(query);
        if (subscription == null) return NotFound();
        var resource = SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(subscription);
        return Ok(resource);
    }

    [HttpGet("id/{subscriptionId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSubscriptionById([FromRoute] int subscriptionId)
    {
        var query = new GetSubscriptionById(subscriptionId);
        var subscription = await subscriptionQueryService.Handle(query);
        if (subscription == null) return NotFound();
        var resource = SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(subscription);
        return Ok(resource);
    }
}