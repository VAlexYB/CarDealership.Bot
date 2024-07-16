using CarDealership.Bot.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Bot.DataAccess.Repositories.Impl
{
    public class UserChatRepository : IUserChatRepository
    {
        private readonly CDBotDbContext _context;

        public UserChatRepository(CDBotDbContext context)
        {
            _context = context;
        }

        public async Task AddOrUpdateUserChatMapping(string phoneNumber, long chatId)
        {
            var userChat = await _context.UserChatMappings
                .FirstOrDefaultAsync(uc => uc.PhoneNumber == phoneNumber);

            if (userChat == null)
            {
                userChat = new UserChatMapping { PhoneNumber = phoneNumber, ChatId = chatId };
                await _context.UserChatMappings.AddAsync(userChat);
            }
            else
            {
                userChat.ChatId = chatId;
                _context.UserChatMappings.Update(userChat);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<long?> GetChatIdByPhoneNumber(string phoneNumber)
        {
            var userChat = await _context.UserChatMappings
                .FirstOrDefaultAsync(uc => uc.PhoneNumber == phoneNumber);

            return userChat?.ChatId;
        }

        public async Task<bool> HasProvidedContact(long chatId)
        {
            var hasProvidedContact = await _context.UserChatMappings.AnyAsync(uc => uc.ChatId == chatId);
            return hasProvidedContact;
        }
    }
}
