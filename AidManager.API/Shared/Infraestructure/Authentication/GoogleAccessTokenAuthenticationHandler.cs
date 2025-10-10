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
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.Fail("Missing Authorization Header");

        string authHeader = Request.Headers.Authorization!;
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.Fail("Invalid Authorization Header");

        string accessToken = authHeader["Bearer ".Length..].Trim();
        var userCredential = await googleAuthorization.ValidateToken(accessToken);
        Credential user = await GetUserCredential(userCredential.Token.AccessToken);
        if (user == null)
            AuthenticateResult.Fail("InvalidAccess Token Provided");

        List<Claim> claims = [new(ClaimTypes.NameIdentifier, user!.UserId.ToString())];
        var identity = new ClaimsIdentity(claims, Constant.Scheme);
        return AuthenticateResult.Success(
            new AuthenticationTicket(
                new ClaimsPrincipal(identity), Constant.Scheme));
    }

    private async Task<Credential> GetUserCredential(string accessToken) =>
        await context.Credentials.FirstOrDefaultAsync(c => c.AccessToken == accessToken);
}