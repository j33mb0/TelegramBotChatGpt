using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using TelegramBotChatGpt.Models;

namespace TelegramBotChatGpt.Services
{
    internal class UserSettingsHandler
    {
        ApplicationContext context = new ApplicationContext();

        public async Task<bool> IsSavedUser(long UserId)
        {
            return context.Users.Any(u => u.Id== UserId);
        }

        public async Task SaveNewUser(long UserId)
        {
            await context.Users.AddAsync(new TelegramUser { Id= UserId, TypeResponseMessage = "Voice" });
            await context.SaveChangesAsync();
        }

        public async Task<string> GetUserResponseType(long UserId)
        {
            return context.Users.FirstOrDefault(u => u.Id== UserId).TypeResponseMessage;
        }

        public async Task ChangeUserResponseType(long UserId, string type)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == UserId);
            user.TypeResponseMessage = type;
            await context.SaveChangesAsync();
        }
    }
}
