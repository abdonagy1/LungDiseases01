using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Domain.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.IdentityData.DbContext
{
    public class LungIdentityDbContext:IdentityDbContext<ApplicationUser>
    {
        public LungIdentityDbContext(DbContextOptions<LungIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
           
            builder.Entity<ApplicationUser>().ToTable( "Users");
            builder.Entity<IdentityRole>().ToTable( "Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable( "UserRoles");
        }

    }
}
