using Gss.ApiGateway.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gss.ApiGateway.Services.Identity
{
    public class GssSignInManager : SignInManager<GssUser>
    {
        public GssSignInManager(UserManager<GssUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<GssUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<GssUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<GssUser> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }
    }
}
