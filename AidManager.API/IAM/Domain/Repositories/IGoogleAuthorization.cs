using Google.Apis.Auth.OAuth2;

namespace AidManager.API.Shared.Domain.Repositories;

public interface IGoogleAuthorization
{
    string GetAuthorizationUrl();
    Task<UserCredential> ExchangeCodeForToken(string code);
    Task<UserCredential> ValidateToken(string accessToken);
}