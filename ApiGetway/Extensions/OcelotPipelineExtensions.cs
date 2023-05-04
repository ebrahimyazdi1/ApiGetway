using Ocelot.Errors;
using Ocelot.Infrastructure.RequestData;
using System.Collections.Generic;

namespace Gss.ApiGateway.Extensions
{
    public static class OcelotPipelineExtensions
    {
        public static void AddError(this IRequestScopedDataRepository scopedDataRepository, Error error)
        {
            var errors = scopedDataRepository.Get<List<Error>>("Errors");
            if (errors.IsError)
            {
                scopedDataRepository.Add("Errors", new List<Error>());
                errors = scopedDataRepository.Get<List<Error>>("Errors");
            }
            errors.Data.Add(error);
        }
    }
}
