using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth;
using AidManager.API.Authentication.Domain.Model.Entities;
using AidManager.API.IAM.Domain.Model.Aggregates;

namespace AidManager.API.Shared.Domain.Repositories;

public interface IGoogleAuthorization
{
    string GetAuthorizationUrl();
    Task<UserCredential> ExchangeCodeForToken(string code);
    Task<(Credential? Credential, User? User, GoogleJsonWebSignature.Payload? GoogleData)> ExchangeCodeForTokenWithUserData(string code);
    Task<UserCredential> ValidateToken(string accessToken);
}