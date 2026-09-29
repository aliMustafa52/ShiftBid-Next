using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class CustomProfileService(UserManager<ApplicationUser> userManager) 
    : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await userManager.GetUserAsync(context.Subject);
        
        if (user != null)
        {
            var claims = new List<Claim>
            {
                new("username", user.UserName ?? string.Empty)
            };

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
                // Add OpenID Connect standard email claim
                claims.Add(new Claim("email", user.Email));
            }

            context.IssuedClaims.AddRange(claims);
        }
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await userManager.GetUserAsync(context.Subject);
        context.IsActive = user != null;
    }
}
