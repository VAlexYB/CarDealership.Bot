using CarDealership.Bot.DataAccess.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CarDealership.Bot.DataAccess
{
    public class CDBotDbContext
    {
        private IMongoDatabase _database;
        public CDBotDbContext(IOptions<MongoConnectionOptions> options)
        {
            MongoClient client = new MongoClient(options.Value.ConnectionString);
            _database = client.GetDatabase(options.Value.DBName);
        }
        public IMongoCollection<UserChatMapping> UserChatMappings =>
               _database.GetCollection<UserChatMapping>(nameof(UserChatMapping));
    }
}
