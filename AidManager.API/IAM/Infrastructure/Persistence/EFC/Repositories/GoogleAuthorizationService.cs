using AidManager.API.IAM.Domain.Model.Aggregates;
using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.Shared.Infraestructure.Persistence.EFC.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.EntityFrameworkCore;

namespace AidManager.API.Shared.Infraestructure.Persistence.EFC.Repositories;

public class GoogleAuthorizationService(
    AppDBContext context, IGoogleAuthHelper googleHelper, IConfiguration config) : IGoogleAuthorization
{
    private string RedirectUrl = config["Google:RedirectUri"];

    public string GetAuthorizationUrl() =>
        new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = googleHelper.GetClientSecrets(),
                Scopes = googleHelper.GetScopes(),
                Prompt = "consent"
            }).CreateAuthorizationCodeRequest(RedirectUrl).Build().ToString();

    public async Task<UserCredential> ExchangeCodeForToken(string code)
    {
        var flow = new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = googleHelper.GetClientSecrets(),
                Scopes = googleHelper.GetScopes()
            });

        var token = await flow.ExchangeCodeForTokenAsync(
            "user", code, RedirectUrl, CancellationToken.None);

        var _credential = new Credential
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            ExpiresInSeconds = token.ExpiresInSeconds,
            IdToken = token.IdToken,
            UserId = Guid.NewGuid(),
            IssuedUtc = token.IssuedUtc
        };

        // ⚠️ FALTA AGREGAR A LA BASE DE DATOS
        await context.Credentials.AddAsync(_credential);
        await context.SaveChangesAsync();

        Console.WriteLine($"Credential saved for UserId: {_credential.UserId}");

        return new UserCredential(flow, "user", token);
    }


    public async Task<UserCredential> ValidateToken(string accessToken)
    {
        var _credential = await context.Credentials.FirstOrDefaultAsync(c=>c.AccessToken == accessToken) ?? 
                          throw new UnauthorizedAccessException("No Authentication token found. Please login again");
        var flow = new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = googleHelper.GetClientSecrets(),
                Scopes = googleHelper.GetScopes(),
            });

        var tokenResponse = new TokenResponse
        {
            AccessToken = _credential.AccessToken,
            RefreshToken = _credential.RefreshToken,
            ExpiresInSeconds = _credential.ExpiresInSeconds,
            IdToken = _credential.IdToken,
            IssuedUtc = _credential.IssuedUtc
        };
        return new UserCredential(flow, "user", tokenResponse);
    }
}