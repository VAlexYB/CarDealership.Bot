using CarDealership.Bot.DataAccess.Repositories;
using CarDealership.Bot.DataAccess.Repositories.Impl;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Metrics;
using System.Drawing;

namespace CarDealership.Bot.DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            services.AddScoped<IUserChatRepository, UserChatRepository>();
            
            return services;
        }
    }
}
