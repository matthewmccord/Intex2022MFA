using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intex2022.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Intex2022.Data
{
    public class Intex2022Context : IdentityDbContext<Intex2022User>
    {
        public Intex2022Context(DbContextOptions<Intex2022Context> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
