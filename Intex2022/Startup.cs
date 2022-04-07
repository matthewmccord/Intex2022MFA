using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ML.OnnxRuntime;
using Intex2022.Models;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Intex2022
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential 
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                // requires using Microsoft.AspNetCore.Http;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Increased the Required Password Length to 12 so that there is more security in length
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 1;
            });


            services.AddDbContext<CrashDbContext>(options =>
            {
                //Need to update this to Environment.GetEnvironmentVariable("CrashConnect")
                options.UseMySql("server=database-2.cxahvrtm0kjp.us-east-1.rds.amazonaws.com;port=3306;database=udot;user=admin;password=Adam1234!");
            });

            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                //Need to update this to Environment.GetEnvironmentVariable("IdentityConnect")
                options.UseMySql("server=database-2.cxahvrtm0kjp.us-east-1.rds.amazonaws.com;port=3306;database=udotidentity;user=admin;password=Adam1234!");
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();

            services.AddScoped<ICrashRepository, EFCrashRepository>();

            services.AddSingleton<InferenceSession>(
                        new InferenceSession("wwwroot/best_reg_model.onnx")
                                                    );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("countypage",
                "{countySelect}/Page{pageNum}",
                new { Controller = "Home", action = "CrashDetailsList" });

                endpoints.MapControllerRoute(
                    "Paging",
                    "Page{pageNum}",
                    new { Controller = "Home", action = "CrashDetailsList", pageNum = 1 });

                endpoints.MapControllerRoute(
                    "County", "{countySelect}", new { Controller = "Home", action = "CrashDetailsList" });

                endpoints.MapDefaultControllerRoute();
            });

            //where we seeded the data but we deleted this bc the database is set up
            //IdentitySeedData.EnsurePopulated(app);
        }
    }
}
