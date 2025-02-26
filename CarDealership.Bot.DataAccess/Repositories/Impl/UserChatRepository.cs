using CarDealership.Bot.DataAccess.Entities;
using MongoDB.Driver;

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
                .Find(uc => uc.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync();

            if (userChat == null)
            {
                userChat = new UserChatMapping { PhoneNumber = phoneNumber, ChatId = chatId };
                await _context.UserChatMappings.InsertOneAsync(userChat);
            }
            else
            {
                userChat.ChatId = chatId;
                await _context.UserChatMappings.ReplaceOneAsync(uc => uc.PhoneNumber == phoneNumber, userChat);
            }
        }

        public async Task<long?> GetChatIdByPhoneNumber(string phoneNumber)
        {
            var userChat = await _context.UserChatMappings
                .Find(uc => uc.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync();

            return userChat?.ChatId;
        }

        public async Task<bool> HasProvidedContact(long chatId)
        {
            var count = await _context.UserChatMappings.CountDocumentsAsync(uc => uc.ChatId == chatId);
            return count > 0;
        }
    }
}
