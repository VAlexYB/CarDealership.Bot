namespace CarDealership.Bot.DataAccess.Repositories
{
    public interface IUserChatRepository
    {
        public Task AddOrUpdateUserChatMapping(string phoneNumber, long chatId);

        public Task<long?> GetChatIdByPhoneNumber(string phoneNumber);
        public Task<bool> HasProvidedContact(long chatId);
    }
}
