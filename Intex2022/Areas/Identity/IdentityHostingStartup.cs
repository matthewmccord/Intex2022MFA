using System;
using Intex2022.Areas.Identity.Data;
using Intex2022.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Intex2022.Areas.Identity.IdentityHostingStartup))]
namespace Intex2022.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<Intex2022Context>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("Intex2022ContextConnection")));

                services.AddDefaultIdentity<Intex2022User>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<Intex2022Context>();
            });
        }
    }
}