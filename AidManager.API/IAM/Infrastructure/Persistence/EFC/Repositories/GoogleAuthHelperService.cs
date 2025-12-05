using AidManager.API.Shared.Domain.Repositories;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;

namespace AidManager.API.Shared.Infraestructure.Persistence.EFC.Repositories;

public class GoogleAuthHelperService(IConfiguration config) : IGoogleAuthHelper
{
    public string[] GetScopes()
    {
        return new[]
        {
            "openid",
            "https://www.googleapis.com/auth/userinfo.email",
            "https://www.googleapis.com/auth/userinfo.profile"
        };
    }

    public string ScopeToString() => string.Join(", ", GetScopes());

    public ClientSecrets GetClientSecrets()
    {
        string clientId = config["Google:ClientId"];
        string clientSecret = config["Google:ClientSecret"];
        return new() { ClientId = clientId, ClientSecret = clientSecret };
    }
}