using AidManager.API.Authentication.Domain.Services;
using AidManager.API.IAM.Application.Internal.OutboundServices;
using AidManager.API.IAM.Domain.Model.Queries;
using AidManager.API.IAM.Domain.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AidManager.API.IAM.Infrastructure.Pipeline.Middleware.Components;

public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IUserIAMQueryService userQueryService,
        ITokenService tokenService
    )
    {
        try
        {
            Console.WriteLine("Entering InvokeAsync");
            var path = context.Request.Path.Value;
            
            // Check if the endpoint has [AllowAnonymous] attribute
            var endpoint = context.GetEndpoint();
            var allowAnonymousAttribute = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>();
            var customAllowAnonymous = endpoint?.Metadata?.GetMetadata<AidManager.API.IAM.Infrastructure.Pipeline.Middleware.Attributes.AllowAnonymousAttribute>();
            
            var allowAnonymous = allowAnonymousAttribute != null || 
                                 customAllowAnonymous != null ||
                                 path.Equals("/api/v1/authentication/sign-up", StringComparison.OrdinalIgnoreCase) ||
                                 path.Equals("/api/v1/authentication/sign-in", StringComparison.OrdinalIgnoreCase) ||
                                 path.Equals("/api/v1/users/sign-up", StringComparison.OrdinalIgnoreCase) ||
                                 path.Contains("/swagger", StringComparison.OrdinalIgnoreCase);

            Console.WriteLine($"Path: {path}, Allow Anonymous: {allowAnonymous}");
            
            if (allowAnonymous)
            {
                Console.WriteLine("Skipping Authorization");
                await next(context);
                return;
            }

            Console.WriteLine("Checking Authorization");
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?
                .Split(" ").Last();
            
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("No token provided");
                throw new Exception("No authorization token provided");
            }
            
            var userId = await tokenService.ValidateToken(token);
            if (userId == null)
            {
                Console.WriteLine("Invalid token");
                throw new Exception("Invalid token");
            }
            
            var getUserByIdQuery = new GetUserIAMByIdQuery(userId.Value);
            var user = await userQueryService.Handle(getUserByIdQuery);
            if (user == null)
            {
                Console.WriteLine("User not found");
                throw new Exception("User not found");
            }
            
            Console.WriteLine($"Authorization successful for user: {user.Username}");
            context.Items["UserAuth"] = user;
            Console.WriteLine("Continuing with Middleware pipeline");
            await next(context);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Authorization failed: {e.Message}");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(e.Message);
        }
    }
}
