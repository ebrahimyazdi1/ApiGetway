using Gss.ApiGateway.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Gss.ApiGateway.Services.Identity
{
    public class GssUserManager : UserManager<GssUser>
    {
        public GssUserManager(IUserStore<GssUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<GssUser> passwordHasher,
            IEnumerable<IUserValidator<GssUser>> userValidators,
            IEnumerable<IPasswordValidator<GssUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<GssUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}
