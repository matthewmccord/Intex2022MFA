//commented out because the Seed Data for the Identity Database should already be in there and we can create users now.

//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Intex2022.Models
//{
//    public static class IdentitySeedData
//    {
//        private const string adminUser = "group37";
//        private const string adminPassword = "SecurePasswordFor37!";

//        private const string adminUserMFA = "MFAgroup37";
//        private const string adminPasswordMFA = "MFASecurePasswordFor37!";

//        public static async void EnsurePopulated(IApplicationBuilder app)
//        {
//            AppIdentityDbContext _context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<AppIdentityDbContext>();

//            if (_context.Database.GetPendingMigrations().Any())
//            {
//                _context.Database.Migrate();
//            }

//            UserManager<IdentityUser> userManager = app.ApplicationServices.CreateScope().ServiceProvider
//                .GetRequiredService<UserManager<IdentityUser>>();

//            IdentityUser user = await userManager.FindByIdAsync(adminUser);

//            IdentityUser user2 = await userManager.FindByIdAsync(adminUserMFA);

//            if (user== null)
//            {
//                user = new IdentityUser(adminUser);

//                user.Email = "admin@yeet.com";
//                user.PhoneNumber = "555-1234";

//                await userManager.CreateAsync(user, adminPassword);

                

//            }

//            if (user2 == null)
//            {
//                user2 = new IdentityUser(adminUserMFA);
//                user2.Email = "group37wells@gmail.com";
//                user2.PhoneNumber = "425-628-1716";

//                await userManager.CreateAsync(user2, adminPasswordMFA);



//            }

//        }
//    }
//}
