using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Stl.Fusion.Authentication;

public class LoginData : IdentityUser
{

    public LoginData() {}

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
        base.Email = email;
    }
}
