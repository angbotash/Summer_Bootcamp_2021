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
    public class MessageController : Controller
    {
        private readonly ApplicationContext dbContext;
        private readonly UserManager<User> userManager;
        private readonly IEncryptionService encryptionService;

        public MessageController(UserManager<User> userManager, ApplicationContext dbContext, IEncryptionService encryptionService)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.encryptionService = encryptionService;
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var message = dbContext.Messages.FirstOrDefault(x => x.Id == id);
            var user = userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

            if (message is null || message.SenderId != user.Id)
            {
                return NotFound();
            }

            return View(new EditMessageViewModel
            {
                Id = message.Id,
                Message = encryptionService.Decrypt(message, user.Key).Data,
            });
        }

        [HttpPost]
        public IActionResult Edit(EditMessageViewModel model)
        {
            var message = dbContext.Messages.FirstOrDefault(x => x.Id == model.Id);
            message.Data = model.Message;
            var user = userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

            encryptionService.Encrypt(message, user.Key);

            dbContext.SaveChanges();

            return RedirectToAction("Index", "Chat", new { id = message.RecipientId });
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            var user = userManager.Users.FirstOrDefault(user => user.UserName == User.Identity.Name);
            var message = dbContext.Messages.FirstOrDefault(message => message.Id == id);

            if (message is null || user is null || message.SenderId != user.Id)
            {
                return NotFound();
            }

            dbContext.Messages.Remove(message);
            dbContext.SaveChanges();
            return RedirectToAction("Index", "Chat", new { id = message.RecipientId });
        }
    }
}
