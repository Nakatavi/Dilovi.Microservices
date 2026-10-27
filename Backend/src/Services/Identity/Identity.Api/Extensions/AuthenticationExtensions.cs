using System.Text;
using Identity.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection("Jwt");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SigningKey"]!)),
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = "sub",
                    RoleClaimType = "role"
                };
                options.Events = new JwtBearerEvents { OnTokenValidated = ValidateSessionAsync };
            });

        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        return services;
    }

    private static async Task ValidateSessionAsync(TokenValidatedContext context)
    {
        var userId = context.Principal?.FindFirst("sub")?.Value;
        var stamp = context.Principal?.FindFirst("security_stamp")?.Value;

        if (!Guid.TryParse(userId, out var id))
        {
            context.Fail("Invalid token.");
            return;
        }

        var users = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var user = await users.GetByIdAsync(id, context.HttpContext.RequestAborted);

        if (user is null || user.Status != UserStatus.Active || user.SecurityStamp.ToString() != stamp)
            context.Fail("Session has ended.");

        var membershipClaim = context.Principal?.FindFirst("membership_id")?.Value;
        if (membershipClaim is null)
            return;

        var memberships = context.HttpContext.RequestServices.GetRequiredService<IMembershipRepository>();
        var available = await memberships.GetAvailableAsync(id, context.HttpContext.RequestAborted);

        if (!Guid.TryParse(membershipClaim, out var membershipId) || available.All(m => m.MembershipId != membershipId))
            context.Fail("Membership is no longer available.");
    }
}