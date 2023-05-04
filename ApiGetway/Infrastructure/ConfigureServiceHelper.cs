using Gss.ApiGateway.Data.DataSeed;
using Gss.ApiGateway.Data;
using Gss.ApiGateway.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;

namespace ApiGetway.Infrastructure
{
    public  class ConfigureServiceHelper
    {
        public  IConfiguration Configuration { get; }
        public  IWebHostEnvironment HostingEnvironment { get; }
        public ConfigureServiceHelper(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostingEnvironment=environment;
        }
          
        public  void AddIdentity(IServiceCollection services)
        {
            services.AddIdentity<GssUser, GssRole>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = false;
            })
                    .AddEntityFrameworkStores<GssDbContext>()
                    .AddUserManager<GssDbContext>()
                    .AddRoleManager<GssDbContext>()
                    .AddSignInManager<GssDbContext>()
                    .AddDefaultTokenProviders();
        }

        public  void AddDataSeeders(IServiceCollection services)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                services.AddScoped<IDataSeeder, IdentityDataSeeder>();
            }
            services.AddScoped<IDataSeeder, OcelotDataSeeder>();
        }
    }
}
