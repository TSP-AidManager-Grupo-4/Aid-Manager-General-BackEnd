using AidManager.API.UserProfile.Domain.Model.Commands;
using AidManager.API.UserProfile.Interfaces.REST.Resources;

namespace AidManager.API.UserProfile.Interfaces.REST.Transform;

public static class CompleteOAuthUserCommandFromResourceAssembler
{
    public static CompleteOAuthUserCommand ToCommandFromResource(int userId, string email, CompleteOAuthUserResource resource)
    {
        return new CompleteOAuthUserCommand(
            UserId: userId,
            FirstName: resource.FirstName,
            LastName: resource.LastName,
            Email: email,
            ProfileImg: resource.ProfileImg,
            Role: resource.Role,
            CompanyName: resource.CompanyName,
            CompanyEmail: resource.CompanyEmail,
            CompanyCountry: resource.CompanyCountry,
            CompanyCode: resource.CompanyCode
        );
    }
}
