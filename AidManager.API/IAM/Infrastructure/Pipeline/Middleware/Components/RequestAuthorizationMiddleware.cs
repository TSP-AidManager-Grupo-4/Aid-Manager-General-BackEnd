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
                    var dbContext = context.RequestServices.GetRequiredService<AppDBContext>();
                    
                    var credential = await dbContext.Credentials
                        .FirstOrDefaultAsync(c => c.AccessToken == token);
                
                    if (credential == null)
                    {
                        Console.WriteLine("Invalid Google OAuth token");
                        throw new Exception("Invalid Google OAuth token");
                    }
                
                    // Find the corresponding UserAuth by matching email
                    var user = await dbContext.Set<AidManager.API.Authentication.Domain.Model.Entities.User>()
                        .FirstOrDefaultAsync(u => u.Id == credential.UserId);
                        
                    if (user == null)
                    {
                        Console.WriteLine($"User not found for credential UserId: {credential.UserId}");
                        throw new Exception("User not found for OAuth token");
                    }
                    
                    var userAuth = await dbContext.Set<AidManager.API.IAM.Domain.Model.Aggregates.UserAuth>()
                        .FirstOrDefaultAsync(ua => ua.Username == user.Email);
                    
                    if (userAuth == null)
                    {
                        // OAuth user hasn't completed setup yet - only allow access to complete-oauth endpoint
                        Console.WriteLine($"UserAuth not found for email: {user.Email} - OAuth setup incomplete");
                        
                        // Store minimal info for complete-oauth endpoint
                        context.Items["OAuthUserId"] = user.Id;
                        context.Items["OAuthEmail"] = user.Email;
                        
                        // Only allow complete-oauth endpoint without UserAuth
                        if (!path.Contains("/complete-oauth", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new Exception("User authentication record not found. Please complete OAuth setup by calling /complete-oauth endpoint.");
                        }
                        
                        Console.WriteLine($"Allowing access to complete-oauth endpoint for incomplete OAuth user: {user.Email}");
                    }
                    else
                    {
                        Console.WriteLine($"Google OAuth Authorization successful for UserId: {credential.UserId}, Username: {userAuth.Username}");
                        context.Items["UserAuth"] = userAuth;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Google OAuth validation failed: {ex.Message}");
                    throw new Exception($"Invalid Google OAuth token: {ex.Message}");
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
