namespace Gss.ApiGateway.Infrastructure.Ocelot.SqlServerProvider
{
    using global::Ocelot.Errors;
    public class SetConfigInSqlServerError : Error
    {
        public SetConfigInSqlServerError(string s, int httpStatusCode)
            : base(s, OcelotErrorCode.UnknownError, httpStatusCode)
        {
        }
    }
}
