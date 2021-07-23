using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SummerBootCampTask.Contexts;
using SummerBootCampTask.CoreModels;
using SummerBootCampTask.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext dbContext;
        private readonly UserManager<User> userManager;

        public HomeController(UserManager<User> userManager, ApplicationContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = new HomeViewModel
            {
                FriendRequests = new List<FriendRequestViewModel>(),
                Friends = new List<UserViewModel>(),
                User = new UserViewModel()
            };

            var user = userManager.Users.FirstOrDefault(user => user.Email == User.Identity.Name);

            if (user is null)
            {
                return NotFound();
            }

            result.User = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                UserIdentifier = user.UserIdentifier,
                PrivateProfile = user.PrivateProfile
            };

            var allUsers = userManager.Users.ToList();
            var requests = dbContext.UserFriends.Where(x => x.FriendId == user.Id && !x.IsVerified);

            foreach (var request in requests)
            {
                var friend = allUsers.FirstOrDefault(x => x.Id == request.UserId);

                result.FriendRequests.Add(new FriendRequestViewModel
                {
                    Id = friend.Id,
                    Name = friend.Name
                });
            }

            var areFriends = dbContext.UserFriends.Where(x => (x.FriendId == user.Id || x.UserId == user.Id) && x.IsVerified).ToList();

            foreach (var friend in areFriends)
            {
                var oneFriend = allUsers.FirstOrDefault(x => (x.Id == friend.UserId || x.Id == friend.FriendId) && x.Id != user.Id);

                result.Friends.Add(new UserViewModel
                {
                    Id = oneFriend.Id,
                    Email = oneFriend.Email,
                    Name = oneFriend.Name,
                    UserIdentifier = oneFriend.UserIdentifier,
                    PrivateProfile = oneFriend.PrivateProfile,
                });
            }

            return View(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
