using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intex2022.Models.ViewModels;

namespace Intex2022.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> um, SignInManager<IdentityUser> sim)
        {
            userManager = um;
            signInManager = sim;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByNameAsync(loginModel.Username);

                if (user != null)
                {
                    await signInManager.SignOutAsync();

                    if ((await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false)).Succeeded)
                    {
                        return Redirect("/Home/CrashDetailsList");
                    }
                }
            }

            ModelState.AddModelError("", "invalid name or password");
            return View(loginModel);
        }

        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();

            return Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationModel registrationModel)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = registrationModel.Username, Email = registrationModel.Email, PhoneNumber = registrationModel.phoneNumber };
                var result = await userManager.CreateAsync(user, registrationModel.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                  
            }
            return View(registrationModel);
        }


    }
}
