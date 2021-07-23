using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationContext dbContext;
        private readonly UserManager<User> userManager;

        public UserController(UserManager<User> userManager, ApplicationContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        [HttpGet("Search")]
        public IActionResult Search()
        {
            var users = userManager.Users.ToList();

            if (User.Identity.IsAuthenticated)
            {
                users = users.Where(x => !x.PrivateProfile && x.Email != User.Identity.Name).ToList();
            }

            return View(users);
        }

        [HttpPost("Search")]
        public IActionResult Search(string searchString)
        {
            var user = userManager.Users.Where(x => x.Email != User.Identity.Name);

            if (string.IsNullOrEmpty(searchString))
            {
                return View(user.Where(x => !x.PrivateProfile).ToList());
            }

            if (searchString != null)
            {
                user = user.Where(user => user.UserIdentifier == searchString);

                if (user is null)
                {
                    return NotFound();
                }
            }

            return View(user.ToList());
        }

        [HttpGet("Profile")]
        public IActionResult Profile()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = userManager.Users.FirstOrDefault(user => user.Email == User.Identity.Name);

                if (user is null)
                {
                    return NotFound();
                }

                var result = new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    UserIdentifier = user.UserIdentifier,
                    PrivateProfile = user.PrivateProfile
                };
                return View(result);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("Edit")]
        public IActionResult Edit()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = userManager.Users.FirstOrDefault(user => user.Email == User.Identity.Name);

                if (user is null)
                {
                    return NotFound();
                }

                EditUserViewModel model = new EditUserViewModel
                {
                    Name = user.Name,
                    PrivateProfile = user.PrivateProfile
                };

                return View(model);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("Edit")]
        [Authorize]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.Users.FirstOrDefault(user => user.Email == User.Identity.Name);

                if (user != null)
                {
                    user.Name = model.Name;
                    user.PrivateProfile = model.PrivateProfile;

                    var result = await userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
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
            }

            return View(model);
        }

        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.Users.FirstOrDefault(user => user.Email == User.Identity.Name);

                if (user != null)
                {
                    IdentityResult result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
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
                    ModelState.AddModelError(string.Empty, "User not found");
                }
            }

            return View(model);
        }
    }
}
