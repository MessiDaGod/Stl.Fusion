using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Stl.Versioning;

namespace Stl.Fusion.EntityFramework.Authentication;

[Table("LoginData")]
[Index(nameof(Email))]
public class DbLoginData<TDbUserId> : IHasId<TDbUserId>, IHasVersion<long>
    where TDbUserId : notnull
{
    private readonly NewtonsoftJsonSerialized<ImmutableDictionary<string, string>> _claims =
        NewtonsoftJsonSerialized.New(ImmutableDictionary<string, string>.Empty);

    [Column("DbLoginDataId")]
    [Key] public TDbUserId Id { get; set; } = default!;
    [ConcurrencyCheck] public long Version { get; set; }
    public string? Email { get; set; }
    [Column(TypeName = "varchar(MAX)")]
    public string? PasswordEncrypted { get; set; }
    [Column(TypeName = "varchar(MAX)")]
    public string? UsernameEncrypted { get; set; }

    [MinLength(3)]
    public string Name { get; set; } = "";

    public string ClaimsJson {
        get => _claims.Data;
        set => _claims.Data = value;
    }

    [NotMapped]
    public ImmutableDictionary<string, string> Claims {
        get => _claims.Value;
        set => _claims.Value = value;
    }

    public List<DbUserIdentity<TDbUserId>> Identities { get; } = new();
}
