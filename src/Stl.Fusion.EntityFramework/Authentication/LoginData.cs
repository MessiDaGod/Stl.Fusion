using System.Globalization;
using System.Security;
using System.Security.Claims;
using Stl.Versioning;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Stl;

namespace Stl.Fusion.Authentication;
public class LoginData : IdentityUser
{

    public LoginData() {}
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    [DataMember]
    public string? Email { get; set; }
    [DataMember]
    [Column(TypeName = "varchar(MAX)")]
    public string? UsernameEncrypted { get; set; }
    [DataMember]
    [Column(TypeName = "varchar(MAX)")]
    public string? PasswordEncrypted { get; set; }

    public LoginData(string username, string password, string email)
    {
        this.UsernameEncrypted = username;
        this.PasswordEncrypted = password;
        this.Email = email;
    }
}


public record LongKeyedEntity : IHasId<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
}
