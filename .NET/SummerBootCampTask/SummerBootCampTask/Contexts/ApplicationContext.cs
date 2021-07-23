using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SummerBootCampTask.CoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask.Contexts
{
    public class ApplicationContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<UserFriend> UserFriends { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        // Установка двойного ключа для таблицы UserFriends
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserFriend>().HasKey(k => new { k.UserId, k.FriendId });
        }
    }
}
