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
//using Amazon;
//using Amazon.SecretsManager;
//using Amazon.SecretsManager.Model;
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

        //public static void GetSecret()
        //{
        //    string secretName = "SecretForDatabase";
        //    string region = "us-east-1";
        //    string secret = "";

        //    MemoryStream memoryStream = new MemoryStream();

        //    IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

        //    GetSecretValueRequest request = new GetSecretValueRequest();
        //    request.SecretId = secretName;
        //    request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.

        //    GetSecretValueResponse response = null;

        //    // In this sample we only handle the specific exceptions for the 'GetSecretValue' API.
        //    // See https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
        //    // We rethrow the exception by default.

        //    try
        //    {
        //        response = client.GetSecretValueAsync(request).Result;
        //    }
        //    catch (DecryptionFailureException e)
        //    {
        //        // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
        //        // Deal with the exception here, and/or rethrow at your discretion.
        //        throw;
        //    }
        //    catch (InternalServiceErrorException e)
        //    {
        //        // An error occurred on the server side.
        //        // Deal with the exception here, and/or rethrow at your discretion.
        //        throw;
        //    }
        //    catch (InvalidParameterException e)
        //    {
        //        // You provided an invalid value for a parameter.
        //        // Deal with the exception here, and/or rethrow at your discretion
        //        throw;
        //    }
        //    catch (InvalidRequestException e)
        //    {
        //        // You provided a parameter value that is not valid for the current state of the resource.
        //        // Deal with the exception here, and/or rethrow at your discretion.
        //        throw;
        //    }
        //    catch (ResourceNotFoundException e)
        //    {
        //        // We can't find the resource that you asked for.
        //        // Deal with the exception here, and/or rethrow at your discretion.
        //        throw;
        //    }
        //    catch (System.AggregateException ae)
        //    {
        //        // More than one of the above exceptions were triggered.
        //        // Deal with the exception here, and/or rethrow at your discretion.
        //        throw;
        //    }

        //    // Decrypts secret using the associated KMS key.
        //    // Depending on whether the secret is a string or binary, one of these fields will be populated.
        //    if (response.SecretString != null)
        //    {
        //        secret = response.SecretString;
        //    }
        //    else
        //    {
        //        memoryStream = response.SecretBinary;
        //        StreamReader reader = new StreamReader(memoryStream);
        //        string decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
        //    }

        //    // Your code goes here.
        //}

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
                // Increased the Required Password Length to 10
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 1;
            });



            services.AddDbContext<CrashDbContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionStrings:CrashDbConnection"]);
            });

            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionStrings:IdentityConnection"]);
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



                //Need to do something with different endpoints or different something for different authorization levels





                endpoints.MapDefaultControllerRoute();

                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");

                //endpoints.MapControllerRoute("/admin/{*catchall}", "/Admin/Index");
            });

            IdentitySeedData.EnsurePopulated(app);
        }
    }
}
