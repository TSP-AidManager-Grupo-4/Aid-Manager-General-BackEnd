using System.Security.Claims;
using System.Text.Encodings.Web;
using AidManager.API.IAM.Domain.Model.Aggregates;
using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.Shared.Infraestructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AidManager.API.Shared.Infraestructure.Authentication;

public class GoogleAccessTokenAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> 
    options, ILoggerFactory logger, UrlEncoder encoder, TimeProvider timeProvider, 
    IGoogleAuthorization googleAuthorization, AppDBContext context) :
    AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly TimeProvider timeProvider = timeProvider;
    
    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check if the endpoint allows anonymous access
        var endpoint = Context.GetEndpoint();
        var allowAnonymousAttribute = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>();
        var customAllowAnonymous = endpoint?.Metadata?.GetMetadata<AidManager.API.IAM.Infrastructure.Pipeline.Middleware.Attributes.AllowAnonymousAttribute>();
        
        if (allowAnonymousAttribute != null || customAllowAnonymous != null)
        {
            // Endpoint allows anonymous, don't try to authenticate
            return AuthenticateResult.NoResult();
        }

        // If user is already authenticated by JWT middleware, skip Google authentication
        if (Context.Items.ContainsKey("UserAuth"))
        {
            var userAuth = Context.Items["UserAuth"];
            if (userAuth != null)
            {
                // User already authenticated by JWT, create claims from that
                List<Claim> jwtClaims = [new(ClaimTypes.NameIdentifier, "authenticated")];
                var jwtIdentity = new ClaimsIdentity(jwtClaims, Constant.Scheme);
                return AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new ClaimsPrincipal(jwtIdentity), Constant.Scheme));
            }
        }

        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.NoResult();

        string authHeader = Request.Headers.Authorization!;
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.NoResult();

        string accessToken = authHeader["Bearer ".Length..].Trim();
        
        // Check if this is a JWT token (contains two dots) vs Google token
        // JWT format: header.payload.signature
        // THIS CHECK MUST BE BEFORE calling googleAuthorization.ValidateToken
        if (accessToken.Split('.').Length == 3)
        {
            // This is likely a JWT token, not a Google OAuth token
            // Let the JWT middleware handle it
            return AuthenticateResult.NoResult();
        }
        
        // Only try to validate Google OAuth tokens
        try
        {
            var userCredential = await googleAuthorization.ValidateToken(accessToken);
            Credential user = await GetUserCredential(userCredential.Token.AccessToken);
            if (user == null)
                return AuthenticateResult.Fail("Invalid Access Token Provided");

            List<Claim> claims = [new(ClaimTypes.NameIdentifier, user!.UserId.ToString())];
            var identity = new ClaimsIdentity(claims, Constant.Scheme);
            return AuthenticateResult.Success(
                new AuthenticationTicket(
                    new ClaimsPrincipal(identity), Constant.Scheme));
        }
        catch (Exception)
        {
            // If validation fails, return NoResult to let other authentication handlers try
            return AuthenticateResult.NoResult();
        }
    }

    private async Task<Credential> GetUserCredential(string accessToken) =>
        await context.Credentials.FirstOrDefaultAsync(c => c.AccessToken == accessToken);
}