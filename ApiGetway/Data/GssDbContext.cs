using Gss.ApiGateway.Models.Identity;
using Gss.ApiGateway.Models.Ocelot;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gss.ApiGateway.Data
{
    public class GssDbContext : IdentityDbContext<GssUser, GssRole, string>
    {
        private const string IdentitySchema = "Id";
        public GssDbContext(DbContextOptions<GssDbContext> options) : base(options)
        {
        }

        public DbSet<OcelotConfigEntity> OcelotFileConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<GssRole>(b =>
            {
                b.ToTable("Roles", IdentitySchema);
            });

            builder.Entity<GssUser>(b =>
            {
                b.ToTable("Users", IdentitySchema);
            });

            builder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("UserClaims", IdentitySchema);
            });

            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("UserLogins", IdentitySchema);
            });

            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("UserTokens", IdentitySchema);
            });

            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("RoleClaims", IdentitySchema);
            });

            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("UserRoles", IdentitySchema);
            });

            builder.Entity<OcelotConfigEntity>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).ValueGeneratedOnAdd();

                b.HasIndex(x => x.Version).IsUnique(true);

                b.Property(x => x.IsActive).HasDefaultValue(true);

                //b.Property(x => x.Payload).IsRequired()
                //    .HasConversion(x => JsonConvert.SerializeObject(x),
                //    x => JsonConvert.DeserializeObject<FileConfiguration>(x));

                b.Property(x => x.PayloadAsString).HasColumnName(nameof(OcelotConfigEntity.Payload));

                b.Property(x => x.Version).IsRequired()
                    .HasConversion(x => x.ToString(),
                    x => Version.Parse(x));
            });

        }
    }
}
