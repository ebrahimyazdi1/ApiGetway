using Gss.ApiGateway.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Gss.ApiGateway.Services.Identity
{
    public class GssRoleManager : RoleManager<GssRole>
    {
        public GssRoleManager(IRoleStore<GssRole> store,
            IEnumerable<IRoleValidator<GssRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<GssRole>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}
