using Google.Apis.Auth.OAuth2;

namespace AidManager.API.Shared.Domain.Repositories;

public interface IGoogleAuthorization
{
    string GetAuthorizationUrl();
    Task<UserCredential> ExchangeCodeFforToken(string code);
    Task<UserCredential> ValidateToken(string accessToken);
}