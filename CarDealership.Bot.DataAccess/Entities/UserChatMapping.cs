using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarDealership.Bot.DataAccess.Entities
{
    public class UserChatMapping
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string PhoneNumber { get; set; }
        public long ChatId { get; set; }
    }
}
