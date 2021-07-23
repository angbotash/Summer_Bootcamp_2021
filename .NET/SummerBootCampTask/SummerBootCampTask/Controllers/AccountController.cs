using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SummerBootCampTask.Contexts;
using SummerBootCampTask.CoreModels;
using SummerBootCampTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SummerBootCampTask.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationContext dbContext;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext dbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.dbContext = dbContext;
        }

        [HttpGet("Login")]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }

            return View(model);
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.Users.FirstOrDefault(user => user.Email == model.Email);

                if (user is null)
                {
                    var random = new Random();

                    User newUser = new User
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        Name = model.Name,
                        Key = random.Next(20),
                        UserIdentifier = random.Next(0, 100000).ToString("D5"),
                        PrivateProfile = model.PrivateProfile
                    };
                    // добавляем пользователя
                    var result = await userManager.CreateAsync(newUser, model.Password);

                    if (result.Succeeded)
                    {
                        // установка куки
                        await signInManager.SignInAsync(newUser, false);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "This email is already in use");
                }

            }
            return View(model);
        }

        [HttpPost("LogOut")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            // удаляем аутентификационные куки
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
