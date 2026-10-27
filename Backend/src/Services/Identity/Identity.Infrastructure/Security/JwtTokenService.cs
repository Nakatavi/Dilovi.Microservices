using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Security;

internal sealed class JwtTokenService(IOptions<JwtOptions> options, TimeProvider timeProvider) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    public AccessToken CreateAccessToken(User user, AvailableMembership? membership)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = now.AddMinutes(_options.AccessTokenLifetimeMinutes);

        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
            [JwtRegisteredClaimNames.Email] = user.Email,
            ["security_stamp"] = user.SecurityStamp.ToString()
        };

        if (user.IsSuperAdmin)
        {
            claims["role"] = "SuperAdmin";
        }
        else if (membership is not null)
        {
            claims["role"] = membership.Role.ToString();
            claims["membership_id"] = membership.MembershipId.ToString();
            claims["tenant_id"] = membership.TenantId.ToString();
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Claims = claims,
            IssuedAt = now,
            NotBefore = now,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return new AccessToken(new JsonWebTokenHandler().CreateToken(descriptor), expiresAt);
    }

    public GeneratedToken GenerateOpaqueToken()
    {
        var token = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));
        return new GeneratedToken(token, HashOpaqueToken(token));
    }

    public string HashOpaqueToken(string opaqueToken)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(opaqueToken)));
}