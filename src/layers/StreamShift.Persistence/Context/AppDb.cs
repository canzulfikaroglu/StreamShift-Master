using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StreamShift.Domain.Entities;

namespace StreamShift.Persistence.Context
{
    public class AppDb : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDb(DbContextOptions options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public DbSet<Transfer> Transfers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("StreamShift");
            base.OnModelCreating(builder);
        }
    }
}