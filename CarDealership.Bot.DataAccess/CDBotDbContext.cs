using CarDealership.Bot.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Bot.DataAccess
{
    public class CDBotDbContext : DbContext
    {
        public CDBotDbContext(DbContextOptions<CDBotDbContext> options) : base(options)
        {
        }
        public DbSet<UserChatMapping> UserChatMappings { get; set; }
    }
}
