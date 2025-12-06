using AidManager.API.IAM.Domain.Model.Aggregates;
using AidManager.API.Shared.Domain.Repositories;
using AidManager.API.Shared.Infraestructure.Persistence.EFC.Configuration;
using AidManager.API.Authentication.Domain.Model.Entities;
using AidManager.API.Authentication.Domain.Model.Commands;
using AidManager.API.UserProfile.Domain.Model.Commands;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;

namespace AidManager.API.Shared.Infraestructure.Persistence.EFC.Repositories;

public class GoogleAuthorizationService(
    AppDBContext context, IGoogleAuthHelper googleHelper, IConfiguration config) : IGoogleAuthorization
{
    private string RedirectUrl = config["Google:RedirectUri"] ?? "http://localhost:8080/api/v1/authorize/callback";

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
            UserId = 0,
            IssuedUtc = token.IssuedUtc
        };

        await context.Credentials.AddAsync(_credential);
        await context.SaveChangesAsync();

        Console.WriteLine($"Credential saved with temp UserId: 0");

        return new UserCredential(flow, "user", token);
    }

    public async Task<(Credential? Credential, User? User, GoogleJsonWebSignature.Payload? GoogleData)> ExchangeCodeForTokenWithUserData(string code)
    {
        try
        {
            // First, exchange code for token
            var flow = new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = googleHelper.GetClientSecrets(),
                    Scopes = googleHelper.GetScopes()
                });

            var token = await flow.ExchangeCodeForTokenAsync(
                "user", code, RedirectUrl, CancellationToken.None);

            // Extract Google user info from ID token
            var googleUser = await GetGoogleUserInfo(token.IdToken);
            
            if (googleUser == null)
            {
                Console.WriteLine("ERROR: Google user info is null");
                return (null, null, null);
            }
            
            // Find or create user in database
            var user = await FindOrCreateUser(googleUser);
            if (user == null)
            {
                Console.WriteLine("ERROR: Failed to find or create user");
                return (null, null, null);
            }
            
            // Now create credential with actual UserId
            var credential = new Credential
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                ExpiresInSeconds = token.ExpiresInSeconds,
                IdToken = token.IdToken,
                UserId = user.Id,
                IssuedUtc = token.IssuedUtc
            };

            await context.Credentials.AddAsync(credential);
            // Check if credential already exists for this user
            var existingCredential = await context.Credentials.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (existingCredential != null)
            {
                existingCredential.AccessToken = token.AccessToken;
                existingCredential.RefreshToken = token.RefreshToken;
                existingCredential.ExpiresInSeconds = token.ExpiresInSeconds;
                existingCredential.IdToken = token.IdToken;
                existingCredential.IssuedUtc = token.IssuedUtc;
                context.Credentials.Update(existingCredential);
                credential = existingCredential;
                Console.WriteLine($"Credential updated for UserId: {user.Id}");
            }
            else
            {
                await context.Credentials.AddAsync(credential);
                Console.WriteLine($"Credential created for UserId: {user.Id}");
            }
            await context.SaveChangesAsync();
            
            return (credential, user, googleUser);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in ExchangeCodeForTokenWithUserData: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return (null, null, null);
        }
    }

    private async Task<GoogleJsonWebSignature.Payload?> GetGoogleUserInfo(string idToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            return payload;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating ID token: {ex.Message}");
            return null;
        }
    }

    private async Task<User?> FindOrCreateUser(GoogleJsonWebSignature.Payload googleUser)
    {
        try
        {
            // Check if user already exists by email
            var existingUser = await context.Set<User>().FirstOrDefaultAsync(u => u.Email == googleUser.Email);
            
            if (existingUser != null)
            {
                Console.WriteLine($"User found: {existingUser.Email}");
                // Update profile image if available and different
                if (!string.IsNullOrEmpty(googleUser.Picture) && existingUser.ProfileImg != googleUser.Picture)
                {
                    existingUser.updateImage(new PatchImageCommand(existingUser.Id, googleUser.Picture));
                    context.Set<User>().Update(existingUser);
                    await context.SaveChangesAsync();
                }
                return existingUser;
            }
            
            // Create placeholder user from Google OAuth - to be completed later
            var email = googleUser.Email ?? "";
            
            Console.WriteLine($"Creating placeholder OAuth user with email: {email}");
            
            // Create minimal user with empty fields - will be filled by /complete-oauth endpoint
            var newUser = new User(new CreateUserCommand(
                FirstName: "",  // Empty - to be filled by frontend
                LastName: "",   // Empty - to be filled by frontend
                Age: 0,
                Email: email,
                Phone: "",
                Password: Guid.NewGuid().ToString(), // Random password for OAuth users
                ProfileImg: googleUser.Picture ?? "",
                Role: -1,  // -1 = unassigned role, to be set by /complete-oauth
                CompanyName: "Pending",  // Placeholder
                CompanyEmail: email,
                CompanyCountry: "Pending",
                TeamRegisterCode: Guid.NewGuid().ToString()
            ));
            
            Console.WriteLine($"Placeholder user object created, adding to context");
            context.Set<User>().Add(newUser);
            
            Console.WriteLine($"Saving placeholder user to database");
            await context.SaveChangesAsync();
            
            Console.WriteLine($"Placeholder OAuth user created: {email}, UserId: {newUser.Id} (needs completion via /complete-oauth)");
            return newUser;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in FindOrCreateUser: {ex.Message}");
            Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
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