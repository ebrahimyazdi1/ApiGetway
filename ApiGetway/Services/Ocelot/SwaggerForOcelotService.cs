using Gss.ApiGateway.Data;
using MMLib.SwaggerForOcelot.Configuration;
using Newtonsoft.Json;
using Ocelot.Configuration;
using System.Data.Entity;
using RouteOptions = MMLib.SwaggerForOcelot.Configuration.RouteOptions;


namespace Gss.ApiGateway.Services.Ocelot
{
    public class SwaggerForOcelotService : ISwaggerForOcelotService
    {
        private readonly GssDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SwaggerForOcelotService(GssDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<SwaggerEndPointOptions> GetSwaggerEndPointOptions()
        {
            var obj = GetLatestActiveRoute();
            if (string.IsNullOrEmpty(obj.PayloadAsString))
            {
                return new List<SwaggerEndPointOptions>();
            }

            var result = JsonConvert.DeserializeObject<dynamic>(obj.PayloadAsString).SwaggerEndPoints;

            if (result is null)
            {
                //I have to do this! Otherwise, it will raise an error and the whole application will be shutdown
                return new List<SwaggerEndPointOptions>() {
                    new SwaggerEndPointOptions {
                        Config = new List<SwaggerEndPointConfig> { new SwaggerEndPointConfig { } } } };
            }

            var routeAsString = JsonConvert.SerializeObject(result);
            var listOfOptions = (List<SwaggerEndPointOptions>)JsonConvert.DeserializeObject<List<SwaggerEndPointOptions>>(routeAsString);
            listOfOptions.Insert(1, new SwaggerEndPointOptions { Key = "", Config = new List<SwaggerEndPointConfig> { new SwaggerEndPointConfig { } } });

            return listOfOptions;
        }

        public List<RouteOptions> GetSwaggerRouteOptions()
        {
            var obj = GetLatestActiveRoute();
            if (string.IsNullOrEmpty(obj.PayloadAsString))
            {
                return new List<RouteOptions>();
            }

            var result = JsonConvert.DeserializeObject<dynamic>(obj.PayloadAsString).Routes;
            if (result is null)
            {
                return new List<RouteOptions>();
            }

            var routeAsString = JsonConvert.SerializeObject(result);
            var listOfOptions = JsonConvert.DeserializeObject<List<RouteOptions>>(routeAsString);
            return listOfOptions;
        }

        public bool UserHasAccessToThisRoute(string swaggerKey)
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims.ToList();
            var obj = GetLatestActiveRoute();
            if (string.IsNullOrEmpty(obj.PayloadAsString))
            {
                return false;
            }
            var routes = JsonConvert.DeserializeObject<dynamic>(obj.PayloadAsString).Routes;
            var results = (List<dynamic>)JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(routes));
            var route = results.FirstOrDefault(x =>
            {
                if (x.SwaggerKey == swaggerKey)
                {
                    var claimDictionary = (Dictionary<string, string>)JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(x.RouteClaimsRequirement));
                    if (claimDictionary == null || claimDictionary.Count == 0)
                    {
                        return true;
                    }
                    var hasClaim = claimDictionary.Any(m => claims.Any(c => m.Key == c.Type && m.Value == c.Value));
                    return hasClaim;
                }
                return false;
            });
            return route != null;
        }

        private Models.Ocelot.OcelotConfigEntity GetLatestActiveRoute()
        {
            return _dbContext.OcelotFileConfigurations.AsNoTracking()
                            .OrderByDescending(x => x.Version).FirstOrDefault(x => x.IsActive);
        }

      
    }
}
