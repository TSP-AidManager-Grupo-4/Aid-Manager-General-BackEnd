using System.ComponentModel.DataAnnotations;

namespace AidManager.API.IAM.Domain.Model.Aggregates;

public class Credential
{
    [Key]
    public Guid UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public long? ExpiresInSeconds { get; set; }
    public string IdToken { get; set; }
    public DateTime IssuedUtc { get; set; }
}