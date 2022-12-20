using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Stl.Fusion.Authentication;
using Stl.Fusion.Authentication.Commands;
using Stl.Fusion.EntityFramework.Internal;
using Stl.Multitenancy;

namespace Stl.Fusion.EntityFramework.Authentication;

// ReSharper disable once TypeParameterCanBeVariant
public interface IDbUserRepo<in TDbContext, TDbUser, TDbUserId>
    where TDbContext : DbContext
    where TDbUser : DbUser<TDbUserId>, new()
    where TDbUserId : notnull
{
    Type UserEntityType { get; }

    // Write methods
    Task<TDbUser> Create(TDbContext dbContext, User user, CancellationToken cancellationToken = default);
    Task<(TDbUser DbUser, bool IsCreated)> GetOrCreateOnSignIn(
        TDbContext dbContext, User user, CancellationToken cancellationToken = default);
    Task Edit(
        TDbContext dbContext, TDbUser dbUser, EditUserCommand command, CancellationToken cancellationToken = default);
    Task Remove(
        TDbContext dbContext, TDbUser dbUser, CancellationToken cancellationToken = default);

    // Read methods
    Task<TDbUser?> Get(Tenant tenant, TDbUserId userId, CancellationToken cancellationToken = default);
    Task<TDbUser?> Get(TDbContext dbContext, TDbUserId userId, bool forUpdate, CancellationToken cancellationToken = default);
    Task<TDbUser?> GetByUserIdentity(
        TDbContext dbContext, UserIdentity userIdentity, bool forUpdate, CancellationToken cancellationToken = default);
}

public class DbUserRepo<TDbContext, TDbUser, TDbUserId> : DbServiceBase<TDbContext>,
    IDbUserRepo<TDbContext, TDbUser, TDbUserId>
    where TDbContext : DbContext
    where TDbUser : DbUser<TDbUserId>, new()
    where TDbUserId : notnull
{
    protected DbAuthService<TDbContext>.Options Options { get; init; }
    protected IDbUserIdHandler<TDbUserId> DbUserIdHandler { get; init; }
    protected IDbEntityResolver<TDbUserId, TDbUser> UserResolver { get; init; }
    protected IDbEntityConverter<TDbUser, User> UserConverter { get; init; }

    public Type UserEntityType => typeof(TDbUser);

    public DbUserRepo(DbAuthService<TDbContext>.Options options, IServiceProvider services)
        : base(services)
    {
        Options = options;
        DbUserIdHandler = services.GetRequiredService<IDbUserIdHandler<TDbUserId>>();
        UserResolver = services.DbEntityResolver<TDbUserId, TDbUser>();
        UserConverter = services.DbEntityConverter<TDbUser, User>();
    }

    // Write methods

    public virtual async Task<TDbUser> Create(
        TDbContext dbContext, User user, CancellationToken cancellationToken = default)
    {
        // Creating "base" dbUser
        var dbUser = new TDbUser() {
            Id = DbUserIdHandler.New(),
            Version = VersionGenerator.NextVersion(),
            Name = user.Name,
            Claims = user.Claims,
        };
        dbContext.Add(dbUser);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        user = user with {
            Id = DbUserIdHandler.Format(dbUser.Id)
        };
        // Updating dbUser from the model to persist user.Identities
        UserConverter.UpdateEntity(user, dbUser);
        dbContext.Update(dbUser);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return dbUser;
    }

    public virtual async Task<(TDbUser DbUser, bool IsCreated)> GetOrCreateOnSignIn(
        TDbContext dbContext, User user, CancellationToken cancellationToken = default)
    {
        TDbUser dbUser;
        if (!user.Id.IsEmpty) {
            dbUser = await Get(dbContext, DbUserIdHandler.Parse(user.Id), false, cancellationToken).ConfigureAwait(false)
                ?? throw Errors.EntityNotFound<TDbUser>();
            return (dbUser, false);
        }

        // No user found, let's create it
        dbUser = await Create(dbContext, user, cancellationToken).ConfigureAwait(false);
        return (dbUser, true);
    }

    public virtual async Task Edit(TDbContext dbContext, TDbUser dbUser, EditUserCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Name != null) {
            dbUser.Name = command.Name;
            dbUser.Version = VersionGenerator.NextVersion(dbUser.Version);
        }
        dbContext.Update(dbUser);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task Remove(
        TDbContext dbContext, TDbUser dbUser, CancellationToken cancellationToken = default)
    {
        await dbContext.Entry(dbUser).Collection(nameof(DbUser<object>.Identities))
            .LoadAsync(cancellationToken).ConfigureAwait(false);
        if (dbUser.Identities.Count > 0)
            dbContext.RemoveRange(dbUser.Identities);
        dbContext.Remove(dbUser);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    // Read methods

    public async Task<TDbUser?> Get(Tenant tenant, TDbUserId userId, CancellationToken cancellationToken = default)
        => await UserResolver.Get(tenant, userId, cancellationToken).ConfigureAwait(false);

    public virtual async Task<TDbUser?> Get(
        TDbContext dbContext, TDbUserId userId, bool forUpdate, CancellationToken cancellationToken = default)
    {
        var dbUsers = forUpdate
            ? dbContext.Set<TDbUser>().ForUpdate()
            : dbContext.Set<TDbUser>();
        var dbUser = await dbUsers
            .SingleOrDefaultAsync(u => Equals(u.Id, userId), cancellationToken)
            .ConfigureAwait(false);
        if (dbUser != null)
            await dbContext.Entry(dbUser).Collection(nameof(DbUser<object>.Identities))
                .LoadAsync(cancellationToken).ConfigureAwait(false);
        return dbUser;
    }

    public virtual async Task<TDbUser?> GetByUserIdentity(
        TDbContext dbContext, UserIdentity userIdentity, bool forUpdate, CancellationToken cancellationToken = default)
    {
        if (!userIdentity.IsValid)
            return null;
        var dbUserIdentities = forUpdate
            ? dbContext.Set<DbUserIdentity<TDbUserId>>().ForUpdate()
            : dbContext.Set<DbUserIdentity<TDbUserId>>();
        var id = userIdentity.Id.Value;
        var dbUserIdentity = await dbUserIdentities
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        if (dbUserIdentity == null)
            return null;
        var user = await Get(dbContext, dbUserIdentity.DbUserId, forUpdate, cancellationToken).ConfigureAwait(false);
        return user;
    }
}


[Serializable()]
[System.Xml.Serialization.XmlRoot(ElementName = "LoginData")]
[Table(nameof(LoginData))]
[Index(nameof(Username))]
public record LoginData : LongKeyedEntity, ILoginData
{
    public LoginData() {}

    private readonly NewtonsoftJsonSerialized<ImmutableDictionary<string, string>> _claims =
        NewtonsoftJsonSerialized.New(ImmutableDictionary<string, string>.Empty);
    public LoginData(string? name, string? username, string? password, string email)
    {
        this.Name = name;
        this.Username = username;
        this.Password = password;
        this.Email = email;
    }
    public string? Name { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? PasswordEncrypted { get; set; }
    public string? UsernameEncrypted { get; set; }
    public bool IsAuthenticated { get; set; }

    [NotMapped, JsonIgnore]
    public ImmutableDictionary<string, string> Claims {
        get => _claims.Value;
        set => _claims.Value = value;
    }
    public virtual LoginData AddLoginData(string? name, string username, string password, string? email)
    {
        var loginData = new LoginData(name, username, email, password);
        return loginData;
    }
}
public interface ILoginData
{
    LoginData AddLoginData(string? name, string username, string password, string? email);
}
//
// public class RegExUtilities
// {
//     public static bool IsValidEmail(string email)
//     {
//         if (string.IsNullOrWhiteSpace(email))
//             return false;
//         try
//         {
//             // Normalize the domain
//             email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
//                                   RegexOptions.None, TimeSpan.FromMilliseconds(200));
//             // Examines the domain part of the email and normalizes it.
//             string DomainMapper(Match match)
//             {
//                 // Use IdnMapping class to convert Unicode domain names.
//                 var idn = new IdnMapping();
//                 // Pull out and process domain name (throws ArgumentException on invalid)
//                 string domainName = idn.GetAscii(match.Groups[2].Value);
//                 return match.Groups[1].Value + domainName;
//             }
//         }
//         catch (RegexMatchTimeoutException e)
//         {
//             return false;
//         }
//         catch (ArgumentException e)
//         {
//             return false;
//         }
//         try
//         {
//             return Regex.IsMatch(email,
//                 @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
//                 RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
//         }
//         catch (RegexMatchTimeoutException)
//         {
//             return false;
//         }
//     }
// }

public record LongKeyedEntity : IHasId<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
}
