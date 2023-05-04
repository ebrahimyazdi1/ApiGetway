using System;

namespace Gss.ApiGateway.Models.Errors
{
    public class OcelotConfigNotSetException : Exception
    {
        public OcelotConfigNotSetException() : base("Ocelot config has not been set")
        {

        }
    }
}
