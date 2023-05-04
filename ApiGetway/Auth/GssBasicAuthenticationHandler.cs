using Gss.ApiGateway.Services.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Ocelot.Infrastructure.RequestData;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Gss.ApiGateway.Auth
{
    public class GssBasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "GssBasicAuthenticationHandler";
        private const string AuthorizationFailed = "Authorization failed";

        private readonly GssUserManager _userManager;
        private readonly GssSignInManager _signInManager;
        private readonly IRequestScopedDataRepository _requestScopedDataRepository;
        public GssBasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, GssSignInManager signInManager,
            GssUserManager userManager, IRequestScopedDataRepository requestScopedDataRepository)
            : base(options, logger, encoder, clock)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _requestScopedDataRepository = requestScopedDataRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Basic Authorization Header");
            }

            try
            {
                var (username, password) = getUserAndPasswordFromHeader();

                var user = await _userManager.FindByNameAsync(username).ConfigureAwait(false);
                if (user == null)
                {
                    return AuthenticateResult.Fail(AuthorizationFailed);
                }

                var signInResult = await _signInManager.PasswordSignInAsync(username, password, false, true).ConfigureAwait(false);
                if (!signInResult.Succeeded)
                {
                    return AuthenticateResult.Fail(AuthorizationFailed);
                }

                var userClaims = (await _userManager.GetClaimsAsync(user).ConfigureAwait(false)).ToList();

                var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                };
                claims.AddRange(userClaims);

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                _requestScopedDataRepository.Add("CurrentUser", user);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception)
            {
                return AuthenticateResult.Fail(AuthorizationFailed);
            }
        }

        private (string username, string password) getUserAndPasswordFromHeader()
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
            return (credentials[0], credentials[1]);
        }
    }
}
