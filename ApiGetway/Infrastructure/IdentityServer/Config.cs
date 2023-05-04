using IdentityServer4.Models;

namespace ApiGetway.Infrastructure.IdentityServer
{
    public static class Config
    {

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId()

            };
        public static IEnumerable<ApiScope> ApiScopes =>

            new ApiScope[]
                  {

                  new ApiScope("api1","api1 scope"),
                   new ApiScope( "api2","api2 scope")
                  };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {

                  new Client
                  {
                  ClientId="client",
                  AllowedGrantTypes=GrantTypes.ClientCredentials,
                  ClientSecrets=
                  {
                      new Secret("secret".Sha256())
                  },
                  AllowedScopes={ "api1"},
                  }
            };
    }

}
