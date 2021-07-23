using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SummerBootCampTask.Contexts;
using SummerBootCampTask.CoreModels;
using SummerBootCampTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask.Controllers
{
    [Authorize]
    public class InvitationController : Controller
    {
        private readonly ApplicationContext dbContext;
        private readonly UserManager<User> userManager;

        public InvitationController(UserManager<User> userManager, ApplicationContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        public IActionResult Invite(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var allUsers = userManager.Users.ToList();
            var user = allUsers.FirstOrDefault(user => user.Email == User.Identity.Name);
            var friend = allUsers.FirstOrDefault(user => user.Id == id);

            if (user is null || friend is null)
            {
                return NotFound();
            }

            var usersAreFriends = dbContext.UserFriends
                .Any(x => (x.UserId == user.Id || x.UserId == friend.Id) && (x.FriendId == user.Id || x.FriendId == friend.Id));

            if (usersAreFriends)
            {
                return View(new UserFriendViewModel { Friend = friend.Name, Message = $"You and {friend.Name} are already friends!" });
            }

            dbContext.UserFriends.Add(new UserFriend
            {
                UserId = user.Id,
                FriendId = friend.Id,
                IsVerified = false
            });

            dbContext.SaveChanges();

            return View(new UserFriendViewModel { Friend = friend.Name, Message = "Your request has been sent!" });
        }

        [HttpPost("Confirm")]
        public IActionResult Confirm(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var user = userManager.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
            var request = dbContext.UserFriends.FirstOrDefault(x => x.UserId == id && x.FriendId == user.Id);

            request.IsVerified = true;

            dbContext.Chats.Add(new Chat
            {
                FirstUserId = user.Id,
                SecondUserId = id.Value,
            });

            dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Delete")]
        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var allUsers = userManager.Users.ToList();
            var user = allUsers.FirstOrDefault(user => user.Email == User.Identity.Name);
            var friend = allUsers.FirstOrDefault(u => u.Id == id);

            if (user is null || friend is null)
            {
                return NotFound();
            }

            var friends = dbContext.UserFriends
                .FirstOrDefault(x => (x.UserId == user.Id || x.UserId == friend.Id) && (x.FriendId == user.Id || x.FriendId == friend.Id) && x.IsVerified);

            if (friends is null)
            {
                return NotFound();
            }

            dbContext.UserFriends.Remove(friends);
            dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
