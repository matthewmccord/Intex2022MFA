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
using Joonasw.AspNetCore.SecurityHeaders;

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

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

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
                options.UseMySql(Environment.GetEnvironmentVariable("CrashConnect"));
            });

            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                //Need to update this to Environment.GetEnvironmentVariable("IdentityConnect")
                options.UseMySql(Environment.GetEnvironmentVariable("IdentityConnect"));
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<ICrashRepository, EFCrashRepository>();

            services.AddRazorPages();
            
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
            else
            {
                app.UseHsts();
            }


            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add(
            //        "Content-Security-Policy",
            //        "script-src 'self'; " +
            //        "style-src 'self'; " +
            //        "img-src 'self'");

            //    await next();
            //});
            // Content Security Policy
            app.UseCsp(csp =>
            {
                // If nothing is mentioned for a resource class, allow from this domain
                csp.ByDefaultAllow
                    .FromSelf();

                // Allow JavaScript from:
                csp.AllowScripts
                    .FromSelf() //This domain
                    .From("localhost:5001");

                // CSS allowed from:
                csp.AllowStyles
                    .FromSelf();

                csp.AllowImages
                    .FromSelf();

                // HTML5 audio and video elemented sources can be from:
                csp.AllowAudioAndVideo
                    .FromNowhere();

                // Contained iframes can be sourced from:
                csp.AllowFrames
                    .FromNowhere(); //Nowhere, no iframes allowed

                // Allow AJAX, WebSocket and EventSource connections to:
                csp.AllowConnections
                    .To("ws://localhost:5001")
                    .To("http://localhost:5001")
                    .ToSelf();

                // Allow fonts to be downloaded from:
                csp.AllowFonts
                    .FromSelf();

                // Allow object, embed, and applet sources from:
                csp.AllowPlugins
                    .FromNowhere();

                // Allow other sites to put this in an iframe?
                csp.AllowFraming
                    .FromNowhere(); // Block framing on other sites, equivalent to X-Frame-Options: DENY

                // Do not block violations, only report
                // This is a good idea while testing your CSP
                // Remove it when you know everything will work
                csp.SetReportOnly();
                // Where should the violation reports be sent to?
                csp.ReportViolationsTo("/csp-report");

                // Do not include the CSP header for requests to the /api endpoints
                csp.OnSendingHeader = context =>
                {
                    context.ShouldNotSend = context.HttpContext.Request.Path.StartsWithSegments("/api");
                    return Task.CompletedTask;
                };
            });

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

                endpoints.MapRazorPages();
                endpoints.MapFallbackToPage("/account/manage", "/Identity/Pages/EnableAuthenticator");
            });

            //where we seeded the data but we deleted this bc the database is set up
            //IdentitySeedData.EnsurePopulated(app);
        }
    }
}
