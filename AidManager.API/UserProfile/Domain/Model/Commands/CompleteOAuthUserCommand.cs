namespace AidManager.API.UserProfile.Domain.Model.Commands;

/// <summary>
/// Command to complete OAuth user profile after login.
/// Supports both manager and team member flows.
/// </summary>
public record CompleteOAuthUserCommand(
    int UserId,
    string FirstName, 
    string LastName, 
    string Email,
    string ProfileImg, 
    int Role,
    // For managers (role = 0): create new company
    string? CompanyName = null,
    string? CompanyEmail = null,
    string? CompanyCountry = null,
    // For team members (role != 0): join existing company
    string? CompanyCode = null
);
