namespace AidManager.API.UserProfile.Interfaces.REST.Resources;

/// <summary>
/// Resource for completing OAuth user profile after login.
/// Supports both manager and team member flows.
/// </summary>
public record CompleteOAuthUserResource(
    string FirstName, 
    string LastName, 
    string ProfileImg, 
    int Role,
    // For managers (role = 0): create new company
    string? CompanyName = null,
    string? CompanyEmail = null,
    string? CompanyCountry = null,
    // For team members (role != 0): join existing company
    string? CompanyCode = null
);
