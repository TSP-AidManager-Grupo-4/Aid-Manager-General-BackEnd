using System.Security.Claims;
using System.Text.Encodings.Web;
using AidManager.API.IAM.Domain.Model.Aggregates;
using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.Shared.Infraestructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserAuth = AidManager.API.IAM.Domain.Model.Aggregates.UserAuth;

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
            return AuthenticateResult.NoResult();
        }

        // Si ya fue autenticado por el middleware, crear claims
        if (Context.Items.ContainsKey("UserAuth"))
        {
            var userAuthData = Context.Items["UserAuth"];
            if (userAuthData != null)
            {
                string userId = userAuthData is UserAuth user 
                    ? user.Id.ToString()
                    : ((dynamic)userAuthData).UserId.ToString();

                List<Claim> claims = [new(ClaimTypes.NameIdentifier, userId)];
                var identity = new ClaimsIdentity(claims, Constant.Scheme);
                return AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new ClaimsPrincipal(identity), Constant.Scheme));
            }
        }

        return AuthenticateResult.NoResult();
    }

    private async Task<Credential> GetUserCredential(string accessToken) =>
        await context.Credentials.FirstOrDefaultAsync(c => c.AccessToken == accessToken);
}
