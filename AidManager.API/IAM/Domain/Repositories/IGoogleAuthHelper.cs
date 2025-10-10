using Google.Apis.Auth.OAuth2;

namespace AidManager.API.Shared.Domain.Repositories;

public interface IGoogleAuthHelper
{
    string[] GetScopes();
    string ScopeToString();
    ClientSecrets GetClientSecrets();
}