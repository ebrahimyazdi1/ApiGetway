using Gss.ApiGateway.Models.Identity;
using Gss.ApiGateway.Services.Identity;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Gss.ApiGateway.Data.DataSeed
{
    public class IdentityDataSeeder : IDataSeeder
    {
        private readonly GssUserManager _userManager;
        private readonly IPasswordHasher<GssUser> _passwordHasher;

        public IdentityDataSeeder(GssUserManager userManager, IPasswordHasher<GssUser> passwordHasher)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedData()
        {
            await SeedUsers();
        }

        private async Task SeedUsers()
        {
            GssUser testUser = await _userManager.FindByNameAsync("test");
            if (testUser == null)
            {
                testUser = new GssUser
                {
                    UserName = "test",
                    EmailConfirmed = true
                };
                testUser.PasswordHash = _passwordHasher.HashPassword(testUser, "test");
                var debugResult = await _userManager.CreateAsync(testUser);
            }
            var claims = await _userManager.GetClaimsAsync(testUser);
            if (!claims.Any(x => x.Type == "todo-claims"))
            {
                await _userManager.AddClaimAsync(testUser, new System.Security.Claims.Claim("todo-claims", "get"));
                await _userManager.AddClaimAsync(testUser, new System.Security.Claims.Claim("todo-claims", "post"));
            }
        }
    }
}
