using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stl.Fusion.Authentication;

public class LoginAudit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string UserId { get; set; } = "0";
    public string Database { get; set; } = "mac";
    public string Client { get; set; } = "Me";
    public string DatabaseType { get; set; } = "Test";
    public DateTime LoginTime { get; set; }
}