using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SummerBootCampTask.Contexts;
using SummerBootCampTask.CoreModels;
using SummerBootCampTask.Models;
using SummerBootCampTask.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationContext dbContext;
        private readonly UserManager<User> userManager;
        private readonly IEncryptionService encryptionService;

        public ChatController(UserManager<User> userManager, ApplicationContext dbContext, IEncryptionService encryptionService) // 
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.encryptionService = encryptionService;
        }

        public IActionResult Index(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var users = userManager.Users.ToList();

            var user = users.FirstOrDefault(user => user.Email == User.Identity.Name);
            var friend = users.FirstOrDefault(user => user.Id == id);

            if (user is null || friend is null)
            {
                return NotFound();
            }

            var userFriend = dbContext.UserFriends
                .FirstOrDefault(x => (x.UserId == user.Id || x.UserId == friend.Id)
                                && (x.FriendId == user.Id || x.FriendId == friend.Id));


            if (userFriend is null)
            {
                return RedirectToAction("Invite", "Invitation", new { id = id });
            }

            var chat = dbContext.Chats.FirstOrDefault(x => (x.FirstUserId == user.Id || x.SecondUserId == user.Id)
                                && (x.FirstUserId == friend.Id || x.SecondUserId == friend.Id) && x.FirstUserId != x.SecondUserId);

            return View(new ChatViewModel
            {
                Id = chat.Id,
                FriendId = friend.Id,
                Messages = GetMessages(chat.Id, user, friend),
                NewMessage = new MessageViewModel(),
            });
        }

        [HttpPost]
        public IActionResult SendMessage(ChatViewModel model)
        {
            var users = userManager.Users.ToList();

            var user = users.FirstOrDefault(user => user.Email == User.Identity.Name);
            var friend = users.FirstOrDefault(user => user.Id == model.FriendId);
            var chat = dbContext.Chats.FirstOrDefault(x => (x.FirstUserId == user.Id || x.SecondUserId == user.Id)
                                && (x.FirstUserId == friend.Id || x.SecondUserId == friend.Id) && x.FirstUserId != x.SecondUserId);

            var message = new Message
            {
                ChatId = chat.Id,
                SenderId = user.Id,
                RecipientId = model.FriendId,
                Data = model.NewMessage.Message.Data,
                DateEdited = DateTime.Now,
            };

            encryptionService.Encrypt(message, user.Key);
            dbContext.Messages.Add(message);
            dbContext.SaveChanges();
            return RedirectToAction("Index", new { id = friend.Id });
        }

        private List<MessageViewModel> GetMessages(int chatId, User user, User friend)
        {
            var messages = dbContext.Messages.Where(x => x.ChatId == chatId).Select(message => new MessageViewModel
            {
                Sender = new ParticipantViewModel
                {
                    Id = message.SenderId,
                    Name = message.SenderId == user.Id ? user.Name : friend.Name,
                },
                Recipient = new ParticipantViewModel
                {
                    Id = message.RecipientId,
                    Name = message.RecipientId == user.Id ? user.Name : friend.Name,
                },
                Message = encryptionService.Decrypt(message, GetKey(message.SenderId, user, friend))
            });

            return messages.ToList();
        }

        private static int GetKey(int senderId, User user, User friend)
        {
            return senderId == user.Id ? user.Key : friend.Key;
        }
    }
}
