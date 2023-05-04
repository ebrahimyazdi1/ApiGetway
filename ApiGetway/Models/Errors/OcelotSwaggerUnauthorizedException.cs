using System;

namespace Gss.ApiGateway.Models.Errors
{
    public class OcelotSwaggerUnauthorizedException : Exception
    {
        public OcelotSwaggerUnauthorizedException(bool shouldAskUserPassword)
        {
            ShouldAskUserPassword = shouldAskUserPassword;
        }

        public bool ShouldAskUserPassword { get; }
    }
}
