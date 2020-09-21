using Foundry.Domain.DbModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Foundry.Identity
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, Role>
    {
        public CustomClaimsPrincipalFactory(UserManager<User> userManager, RoleManager<Role> roleManager
            , IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {

        }
        public async override Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var principal = await base.CreateAsync(user);
            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                        new Claim(ClaimTypes.GivenName, user.FirstName)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.Surname, user.LastName),
                });
            }
            return principal;
        }
    }
}
