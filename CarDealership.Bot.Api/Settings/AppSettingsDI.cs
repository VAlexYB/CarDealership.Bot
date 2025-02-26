using CarDealership.Bot.Api.RabbitMQ;
using CarDealership.Bot.DataAccess;

namespace CarDealership.Bot.Api.Settings
{
    public static class AppSettingsDI
    {
        public static IServiceCollection ConfigureAppSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<MongoConnectionOptions>(config.GetSection(nameof(MongoConnectionOptions)));
            services.Configure<RabbitMQConnectionOptions>(config.GetSection(nameof(RabbitMQConnectionOptions)));

            return services;
        }
    }
}
