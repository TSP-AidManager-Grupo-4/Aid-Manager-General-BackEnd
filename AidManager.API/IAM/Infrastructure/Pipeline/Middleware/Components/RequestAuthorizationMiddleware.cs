using AidManager.API.Authentication.Domain.Services;
using AidManager.API.IAM.Application.Internal.OutboundServices;
using AidManager.API.IAM.Domain.Model.Queries;
using AidManager.API.IAM.Domain.Services;
using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.Shared.Infraestructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AidManager.API.IAM.Infrastructure.Pipeline.Middleware.Components;

public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IUserIAMQueryService userQueryService,
        ITokenService tokenService,
        IGoogleAuthorization googleAuthorization
    )
    {
        try
        {
            Console.WriteLine("Entering InvokeAsync");
            var path = context.Request.Path.Value;
        
            var endpoint = context.GetEndpoint();
            var allowAnonymousAttribute = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>();
            var customAllowAnonymous = endpoint?.Metadata?.GetMetadata<AidManager.API.IAM.Infrastructure.Pipeline.Middleware.Attributes.AllowAnonymousAttribute>();
        
            var allowAnonymous = allowAnonymousAttribute != null || 
                                 customAllowAnonymous != null ||
                                 path.Equals("/api/v1/authentication/sign-up", StringComparison.OrdinalIgnoreCase) ||
                                 path.StartsWith("/api/v1/authorize", StringComparison.OrdinalIgnoreCase) ||
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
        
            // 🔍 Detectar tipo de token
            bool isJwtToken = token.Split('.').Length == 3;
        
            if (isJwtToken)
            {
                // ✅ Validar JWT Token (tu sistema interno)
                Console.WriteLine("Validating JWT Token");
                var userId = await tokenService.ValidateToken(token);
                if (userId == null)
                {
                    Console.WriteLine("Invalid JWT token");
                    throw new Exception("Invalid token");
                }
            
                var getUserByIdQuery = new GetUserIAMByIdQuery(userId.Value);
                var user = await userQueryService.Handle(getUserByIdQuery);
                if (user == null)
                {
                    Console.WriteLine("User not found");
                    throw new Exception("User not found");
                }
            
                Console.WriteLine($"JWT Authorization successful for user: {user.Username}");
                context.Items["UserAuth"] = user;
            }
            else
            {
                // ✅ Validar Google OAuth Token
                Console.WriteLine("Validating Google OAuth Token");
                try
                {
                    var userCredential = await googleAuthorization.ValidateToken(token);
                    var credential = await context.RequestServices
                        .GetRequiredService<AppDBContext>()
                        .Credentials
                        .FirstOrDefaultAsync(c => c.AccessToken == token);
                
                    if (credential == null)
                    {
                        Console.WriteLine("Invalid Google OAuth token");
                        throw new Exception("Invalid Google OAuth token");
                    }
                
                    Console.WriteLine($"Google OAuth Authorization successful for UserId: {credential.UserId}");
                    context.Items["UserAuth"] = new { UserId = credential.UserId, IsGoogleAuth = true };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Google OAuth validation failed: {ex.Message}");
                    throw new Exception("Invalid Google OAuth token");
                }
            }
        
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
