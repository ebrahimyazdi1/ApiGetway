using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gss.ApiGateway.Infrastructure
{
    public class GssGlobalHttpDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GssGlobalHttpDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //do whatever
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
